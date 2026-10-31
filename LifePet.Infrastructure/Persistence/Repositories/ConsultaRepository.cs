using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence.Repositories;

public class ConsultaRepository(LifePetDbContext db) : EfRepository<Consulta>(db), IConsultaRepository
{
    public Task<PagedResult<Consulta>> GetPagedAsync(PagedQuery query, int? petId, bool apenasFuturas, DateTime referencia, CancellationToken cancellationToken = default)
    {
        var source = Set.AsNoTracking().AsQueryable();

        if (petId.HasValue)
            source = source.Where(c => c.PetId == petId.Value);

        if (apenasFuturas)
            source = source.Where(c => c.DataHora >= referencia);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(c => c.Motivo.ToLower().Contains(term) || c.Veterinario.ToLower().Contains(term));
        }

        source = query.SortDirection == "desc"
            ? source.OrderByDescending(c => c.DataHora)
            : source.OrderBy(c => c.DataHora);

        return source.ToPagedAsync(query, cancellationToken);
    }
}
