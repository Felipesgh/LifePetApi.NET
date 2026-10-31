using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence.Repositories;

public class MedicamentoRepository(LifePetDbContext db) : EfRepository<Medicamento>(db), IMedicamentoRepository
{
    public Task<PagedResult<Medicamento>> GetPagedAsync(PagedQuery query, int? petId, bool apenasAtivos, DateTime referencia, CancellationToken cancellationToken = default)
    {
        var source = Set.AsNoTracking().AsQueryable();

        if (petId.HasValue)
            source = source.Where(m => m.PetId == petId.Value);

        if (apenasAtivos)
            source = source.Where(m => m.DataFim == null || m.DataFim >= referencia.Date);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(m => m.Nome.ToLower().Contains(term));
        }

        source = query.SortDirection == "desc"
            ? source.OrderByDescending(m => m.DataInicio)
            : source.OrderBy(m => m.DataInicio);

        return source.ToPagedAsync(query, cancellationToken);
    }
}
