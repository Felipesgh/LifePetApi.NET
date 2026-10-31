using LifePet.Domain.Entities;

namespace LifePet.Domain.Repositories;

public interface IConsultaRepository : IRepository<Consulta>
{
    Task<PagedResult<Consulta>> GetPagedAsync(PagedQuery query, int? petId, bool apenasFuturas, DateTime referencia, CancellationToken cancellationToken = default);
}
