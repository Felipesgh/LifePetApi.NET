using LifePet.Domain.Entities;

namespace LifePet.Domain.Repositories;

public interface IHistoricoRepository : IRepository<Historico>
{
    Task<PagedResult<Historico>> GetPagedAsync(PagedQuery query, int? petId, string? tipo, CancellationToken cancellationToken = default);
}
