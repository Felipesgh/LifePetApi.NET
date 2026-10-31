using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence.Repositories;

public class VacinaRepository(LifePetDbContext db) : EfRepository<Vacina>(db), IVacinaRepository
{
    public Task<PagedResult<Vacina>> GetPagedAsync(PagedQuery query, int? petId, bool apenasAtrasadas, DateTime referencia, CancellationToken cancellationToken = default)
    {
        var source = Set.AsNoTracking().AsQueryable();

        if (petId.HasValue)
            source = source.Where(v => v.PetId == petId.Value);

        if (apenasAtrasadas)
            source = source.Where(v => v.DataProximaDose != null && v.DataProximaDose < referencia.Date);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(v => v.Nome.ToLower().Contains(term));
        }

        source = query.SortBy?.ToLower() switch
        {
            "dataaplicacao" => query.SortDirection == "desc" ? source.OrderByDescending(v => v.DataAplicacao) : source.OrderBy(v => v.DataAplicacao),
            "dataproximadose" => query.SortDirection == "desc" ? source.OrderByDescending(v => v.DataProximaDose) : source.OrderBy(v => v.DataProximaDose),
            _ => query.SortDirection == "desc" ? source.OrderByDescending(v => v.Nome) : source.OrderBy(v => v.Nome)
        };

        return source.ToPagedAsync(query, cancellationToken);
    }
}
