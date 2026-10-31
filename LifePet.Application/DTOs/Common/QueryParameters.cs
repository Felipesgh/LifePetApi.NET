using LifePet.Domain.Repositories;

namespace LifePet.Application.DTOs.Common;

public class QueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
    public string? Search { get; set; }

    public PagedQuery ToPagedQuery()
    {
        var page = Page < 1 ? 1 : Page;
        var pageSize = PageSize is < 1 or > 50 ? 10 : PageSize;
        var direction = string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        return new PagedQuery
        {
            Page = page,
            PageSize = pageSize,
            SortBy = SortBy,
            SortDirection = direction,
            Search = Search
        };
    }
}
