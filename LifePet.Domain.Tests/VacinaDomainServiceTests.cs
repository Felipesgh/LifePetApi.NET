using LifePet.Domain.Entities;
using LifePet.Domain.Services;

namespace LifePet.Domain.Tests;

public class VacinaDomainServiceTests
{
    private readonly VacinaDomainService _sut = new();

    [Fact]
    public void EstaAtrasada_DeveRetornarTrue_QuandoProximaDoseJaPassou()
    {
        // Arrange
        var vacina = new Vacina { DataProximaDose = new DateTime(2026, 1, 1) };

        // Act
        var resultado = _sut.EstaAtrasada(vacina, new DateTime(2026, 10, 26));

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void EstaAtrasada_DeveRetornarFalse_QuandoNaoHaProximaDose()
    {
        // Arrange
        var vacina = new Vacina { DataProximaDose = null };

        // Act
        var resultado = _sut.EstaAtrasada(vacina, DateTime.Today);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void FiltrarAtrasadas_DeveRetornarSomenteVacinasVencidas()
    {
        // Arrange
        var referencia = new DateTime(2026, 10, 26);
        var vacinas = new[]
        {
            new Vacina { Nome = "V8", DataProximaDose = referencia.AddDays(-1) },
            new Vacina { Nome = "Antirrábica", DataProximaDose = referencia.AddDays(10) }
        };

        // Act
        var atrasadas = _sut.FiltrarAtrasadas(vacinas, referencia).ToList();

        // Assert
        Assert.Single(atrasadas);
        Assert.Equal("V8", atrasadas[0].Nome);
    }
}
