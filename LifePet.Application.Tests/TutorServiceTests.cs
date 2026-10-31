using LifePet.Application.DTOs;
using LifePet.Application.DTOs.Common;
using LifePet.Application.Services;
using LifePet.Application.Tests.Fakes;
using LifePet.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace LifePet.Application.Tests;

public class TutorServiceTests
{
    [Fact]
    public async Task CreateAsync_DevePersistirTutorValido()
    {
        // Arrange
        var tutores = new InMemoryTutorRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var sut = new TutorService(tutores, unitOfWork, NullLogger<TutorService>.Instance);

        // Act
        var criado = await sut.CreateAsync(new CreateTutorRequest
        {
            Nome = "Maria Silva",
            Email = "maria@email.com",
            Telefone = "11999999999"
        });

        // Assert
        Assert.Equal(1, criado.Id);
        Assert.Equal("Maria Silva", criado.Nome);
        Assert.Equal(1, unitOfWork.Saves);
        Assert.Single(tutores.Items);
    }

    [Fact]
    public async Task CreateAsync_DeveFalhar_QuandoNomeVazio()
    {
        // Arrange
        var sut = new TutorService(new InMemoryTutorRepository(), new InMemoryUnitOfWork(), NullLogger<TutorService>.Instance);

        // Act
        var act = () => sut.CreateAsync(new CreateTutorRequest { Nome = " ", Email = "a@a.com" });

        // Assert
        await Assert.ThrowsAsync<BusinessException>(act);
    }

    [Fact]
    public async Task GetByIdAsync_DeveLancarNotFound_QuandoNaoExiste()
    {
        // Arrange
        var sut = new TutorService(new InMemoryTutorRepository(), new InMemoryUnitOfWork(), NullLogger<TutorService>.Instance);

        // Act
        var act = () => sut.GetByIdAsync(99);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetAsync_DevePaginarResultados()
    {
        // Arrange
        var tutores = new InMemoryTutorRepository();
        var sut = new TutorService(tutores, new InMemoryUnitOfWork(), NullLogger<TutorService>.Instance);
        await sut.CreateAsync(new CreateTutorRequest { Nome = "Ana", Email = "ana@email.com" });
        await sut.CreateAsync(new CreateTutorRequest { Nome = "Bruno", Email = "bruno@email.com" });

        // Act
        var pagina = await sut.GetAsync(new QueryParameters { Page = 1, PageSize = 1 });

        // Assert
        Assert.Equal(2, pagina.TotalItems);
        Assert.Single(pagina.Items);
        Assert.Equal(2, pagina.TotalPages);
    }
}
