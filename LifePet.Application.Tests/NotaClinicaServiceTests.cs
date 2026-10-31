using LifePet.Application.DTOs;
using LifePet.Application.Services;
using LifePet.Application.Tests.Fakes;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;

namespace LifePet.Application.Tests;

public class NotaClinicaServiceTests
{
    private sealed class InMemoryNotas : INotaClinicaRepository
    {
        private readonly List<NotaClinica> _items = [];

        public Task<NotaClinica?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<PagedResult<NotaClinica>> GetPagedAsync(PagedQuery query, int? petId, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<NotaClinica>
            {
                Items = _items,
                Page = 1,
                PageSize = 10,
                TotalItems = _items.Count
            });

        public Task AddAsync(NotaClinica nota, CancellationToken cancellationToken = default)
        {
            _items.Add(nota);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(NotaClinica nota, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            _items.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task CreateAsync_DeveCriarNotaNoMongoQuandoPetExiste()
    {
        // Arrange
        var pets = new InMemoryPetRepository();
        await pets.AddAsync(new Pet { Nome = "Thor", TutorId = 1 });
        var sut = new NotaClinicaService(new InMemoryNotas(), pets);

        // Act
        var nota = await sut.CreateAsync(new CreateNotaClinicaRequest
        {
            PetId = 1,
            Titulo = "Observação",
            Conteudo = "Pet apresentou melhora.",
            Autor = "Dra. Ana"
        });

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(nota.Id));
        Assert.Equal("Observação", nota.Titulo);
    }

    [Fact]
    public async Task CreateAsync_DeveFalhar_QuandoPetNaoExiste()
    {
        // Arrange
        var sut = new NotaClinicaService(new InMemoryNotas(), new InMemoryPetRepository());

        // Act
        var act = () => sut.CreateAsync(new CreateNotaClinicaRequest
        {
            PetId = 9,
            Titulo = "Nota",
            Conteudo = "Texto"
        });

        // Assert
        await Assert.ThrowsAsync<BusinessException>(act);
    }
}
