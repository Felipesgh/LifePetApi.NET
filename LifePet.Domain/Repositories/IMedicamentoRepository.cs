using LifePet.Domain.Entities;

namespace LifePet.Domain.Repositories;

public interface IMedicamentoRepository : IRepository<Medicamento>
{
    Task<PagedResult<Medicamento>> GetPagedAsync(PagedQuery query, int? petId, bool apenasAtivos, DateTime referencia, CancellationToken cancellationToken = default);
}
