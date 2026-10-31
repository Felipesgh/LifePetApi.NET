using LifePet.Domain.Entities;

namespace LifePet.Domain.Repositories;

public interface IPetRepository : IRepository<Pet>
{
    Task<PagedResult<Pet>> GetPagedAsync(PagedQuery query, int? tutorId, string? especie, CancellationToken cancellationToken = default);
}
