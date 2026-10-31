using LifePet.Domain.Entities;

namespace LifePet.Domain.Services;

public interface IMedicamentoDomainService
{
    bool EstaAtivo(Medicamento medicamento, DateTime referencia);
    IEnumerable<Medicamento> FiltrarAtivos(IEnumerable<Medicamento> medicamentos, DateTime referencia);
}
