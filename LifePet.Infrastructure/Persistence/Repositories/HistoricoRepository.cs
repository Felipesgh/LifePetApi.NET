using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence.Repositories;

public class HistoricoRepository(LifePetDbContext db) : EfRepository<Historico>(db), IHistoricoRepository
{
    public Task<PagedResult<Historico>> GetPagedAsync(PagedQuery query, int? petId, string? tipo, CancellationToken cancellationToken = default)
    {
        var source = Set.AsNoTracking().AsQueryable();

        if (petId.HasValue)
            source = source.Where(h => h.PetId == petId.Value);

        if (!string.IsNullOrWhiteSpace(tipo))
            source = source.Where(h => h.Tipo == tipo);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(h => h.Descricao.ToLower().Contains(term));
        }

        source = query.SortDirection == "asc"
            ? source.OrderBy(h => h.Data)
            : source.OrderByDescending(h => h.Data);

        return source.ToPagedAsync(query, cancellationToken);
    }
}
