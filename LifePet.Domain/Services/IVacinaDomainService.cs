using LifePet.Domain.Entities;

namespace LifePet.Domain.Services;

public interface IVacinaDomainService
{
    bool EstaAtrasada(Vacina vacina, DateTime referencia);
    IEnumerable<Vacina> FiltrarAtrasadas(IEnumerable<Vacina> vacinas, DateTime referencia);
}
