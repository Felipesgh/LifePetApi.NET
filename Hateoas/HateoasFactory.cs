using LifePet.Application.DTOs.Common;
using LifePet.Domain.Repositories;

namespace LifePetApi.Hateoas;

public sealed class HateoasFactory(LinkGenerator linkGenerator, IHttpContextAccessor accessor)
{
    public ResourceDto<T> Wrap<T>(T data, params LinkDto[] links)
        => new() { Data = data, Links = links };

    public PagedResponseDto<T> Page<T>(PagedResult<T> result, string getAction, string controller, object? extraValues, Func<T, LinkDto[]> itemLinks)
    {
        var items = result.Items.Select(item => Wrap(item, itemLinks(item))).ToList();
        var links = new List<LinkDto>
        {
            Action("self", getAction, controller, Merge(extraValues, result.Page, result.PageSize))
        };

        if (result.Page > 1)
            links.Add(Action("prev", getAction, controller, Merge(extraValues, result.Page - 1, result.PageSize)));

        if (result.Page < result.TotalPages)
            links.Add(Action("next", getAction, controller, Merge(extraValues, result.Page + 1, result.PageSize)));

        return new PagedResponseDto<T>
        {
            Items = items,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            Links = links
        };
    }

    public LinkDto Action(string rel, string action, string controller, object? values, string method = "GET")
    {
        var href = linkGenerator.GetUriByAction(accessor.HttpContext!, action, controller, values)
            ?? $" /api/{controller}".Trim();
        return new LinkDto { Rel = rel, Href = href, Method = method };
    }

    private static Dictionary<string, object?> Merge(object? extra, int page, int pageSize)
    {
        var values = extra?
            .GetType()
            .GetProperties()
            .ToDictionary(property => property.Name, property => property.GetValue(extra), StringComparer.OrdinalIgnoreCase)
            ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        values["page"] = page;
        values["pageSize"] = pageSize;
        return values;
    }
}
