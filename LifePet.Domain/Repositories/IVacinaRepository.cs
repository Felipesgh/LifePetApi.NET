using LifePet.Domain.Entities;

namespace LifePet.Domain.Repositories;

public interface IVacinaRepository : IRepository<Vacina>
{
    Task<PagedResult<Vacina>> GetPagedAsync(PagedQuery query, int? petId, bool apenasAtrasadas, DateTime referencia, CancellationToken cancellationToken = default);
}
