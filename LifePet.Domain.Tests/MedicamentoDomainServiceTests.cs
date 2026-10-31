using LifePet.Domain.Entities;
using LifePet.Domain.Services;

namespace LifePet.Domain.Tests;

public class MedicamentoDomainServiceTests
{
    private readonly MedicamentoDomainService _sut = new();

    [Fact]
    public void EstaAtivo_DeveRetornarTrue_QuandoDataFimNula()
    {
        // Arrange
        var medicamento = new Medicamento { DataFim = null };

        // Act
        var resultado = _sut.EstaAtivo(medicamento, DateTime.Today);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void EstaAtivo_DeveRetornarFalse_QuandoTratamentoJaEncerrou()
    {
        // Arrange
        var medicamento = new Medicamento { DataFim = DateTime.Today.AddDays(-2) };

        // Act
        var resultado = _sut.EstaAtivo(medicamento, DateTime.Today);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void FiltrarAtivos_DeveIgnorarMedicamentosEncerrados()
    {
        // Arrange
        var hoje = DateTime.Today;
        var medicamentos = new[]
        {
            new Medicamento { Nome = "Dipirona", DataFim = null },
            new Medicamento { Nome = "Antibiótico", DataFim = hoje.AddDays(-1) }
        };

        // Act
        var ativos = _sut.FiltrarAtivos(medicamentos, hoje).ToList();

        // Assert
        Assert.Single(ativos);
        Assert.Equal("Dipirona", ativos[0].Nome);
    }
}
