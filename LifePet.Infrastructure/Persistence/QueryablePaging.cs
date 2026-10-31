using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence;

internal static class QueryablePaging
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> source,
        PagedQuery query,
        CancellationToken cancellationToken)
    {
        var total = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total
        };
    }
}
