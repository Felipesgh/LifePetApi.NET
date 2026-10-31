using LifePet.Domain.Entities;

namespace LifePet.Domain.Services;

public class MedicamentoDomainService : IMedicamentoDomainService
{
    public bool EstaAtivo(Medicamento medicamento, DateTime referencia)
        => !medicamento.DataFim.HasValue || medicamento.DataFim.Value.Date >= referencia.Date;

    public IEnumerable<Medicamento> FiltrarAtivos(IEnumerable<Medicamento> medicamentos, DateTime referencia)
        => medicamentos.Where(medicamento => EstaAtivo(medicamento, referencia));
}
