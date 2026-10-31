using LifePet.Domain.Entities;

namespace LifePet.Domain.Services;

public class VacinaDomainService : IVacinaDomainService
{
    public bool EstaAtrasada(Vacina vacina, DateTime referencia)
        => vacina.DataProximaDose.HasValue && vacina.DataProximaDose.Value.Date < referencia.Date;

    public IEnumerable<Vacina> FiltrarAtrasadas(IEnumerable<Vacina> vacinas, DateTime referencia)
        => vacinas.Where(vacina => EstaAtrasada(vacina, referencia));
}
