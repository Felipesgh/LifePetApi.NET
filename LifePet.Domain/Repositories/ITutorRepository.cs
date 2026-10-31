using LifePet.Domain.Entities;

namespace LifePet.Domain.Repositories;

public interface ITutorRepository : IRepository<Tutor>
{
    Task<Tutor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<Tutor>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
}
