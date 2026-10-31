using LifePet.Domain.Entities;

namespace LifePet.Domain.Repositories;

public interface INotaClinicaRepository
{
    Task<NotaClinica?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<PagedResult<NotaClinica>> GetPagedAsync(PagedQuery query, int? petId, CancellationToken cancellationToken = default);
    Task AddAsync(NotaClinica nota, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotaClinica nota, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
