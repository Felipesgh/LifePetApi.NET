namespace LifePet.Application.DTOs.Common;

public sealed class LinkDto
{
    public string Rel { get; init; } = string.Empty;
    public string Href { get; init; } = string.Empty;
    public string Method { get; init; } = "GET";
}

public sealed class ResourceDto<T>
{
    public T Data { get; init; } = default!;
    public IReadOnlyList<LinkDto> Links { get; init; } = [];
}

public sealed class PagedResponseDto<T>
{
    public IReadOnlyList<ResourceDto<T>> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public IReadOnlyList<LinkDto> Links { get; init; } = [];
}
