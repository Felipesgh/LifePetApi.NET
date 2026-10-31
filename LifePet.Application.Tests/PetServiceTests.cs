using LifePet.Application.DTOs;
using LifePet.Application.Services;
using LifePet.Application.Tests.Fakes;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;

namespace LifePet.Application.Tests;

public class PetServiceTests
{
    [Fact]
    public async Task CreateAsync_DeveFalhar_QuandoTutorNaoExiste()
    {
        // Arrange
        var sut = new PetService(new InMemoryPetRepository(), new InMemoryTutorRepository(), new InMemoryUnitOfWork(), NullLogger<PetService>.Instance);

        // Act
        var act = () => sut.CreateAsync(new CreatePetRequest { Nome = "Thor", TutorId = 1 });

        // Assert
        await Assert.ThrowsAsync<BusinessException>(act);
    }

    [Fact]
    public async Task CreateAsync_DeveCriarPet_QuandoTutorExiste()
    {
        // Arrange
        var tutores = new InMemoryTutorRepository();
        await tutores.AddAsync(new Tutor { Nome = "Maria", Email = "maria@email.com" });
        var pets = new InMemoryPetRepository();
        var sut = new PetService(pets, tutores, new InMemoryUnitOfWork(), NullLogger<PetService>.Instance);

        // Act
        var pet = await sut.CreateAsync(new CreatePetRequest
        {
            Nome = "Thor",
            Especie = "Cão",
            Raca = "Labrador",
            DataNascimento = new DateTime(2020, 5, 10),
            TutorId = 1
        });

        // Assert
        Assert.Equal("Thor", pet.Nome);
        Assert.Equal(1, pet.TutorId);
        Assert.True(await pets.ExistsAsync(pet.Id));
    }
}
