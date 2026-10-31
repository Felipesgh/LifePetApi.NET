using LifePet.Application.DTOs;
using LifePet.Application.Services;
using LifePet.Application.Tests.Fakes;
using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using LifePet.Domain.Services;

namespace LifePet.Application.Tests;

public class VacinaServiceTests
{
    private sealed class InMemoryVacinas : IVacinaRepository
    {
        private readonly List<Vacina> _items = [];
        private int _nextId = 1;

        public Task<Vacina?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<IReadOnlyList<Vacina>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Vacina>>(_items);

        public Task AddAsync(Vacina entity, CancellationToken cancellationToken = default)
        {
            entity.Id = _nextId++;
            _items.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(Vacina entity) { }

        public void Remove(Vacina entity) => _items.Remove(entity);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.Any(x => x.Id == id));

        public Task<PagedResult<Vacina>> GetPagedAsync(PagedQuery query, int? petId, bool apenasAtrasadas, DateTime referencia, CancellationToken cancellationToken = default)
        {
            IEnumerable<Vacina> filtered = _items;
            if (apenasAtrasadas)
                filtered = filtered.Where(v => v.DataProximaDose < referencia);
            var list = filtered.ToList();
            return Task.FromResult(new PagedResult<Vacina>
            {
                Items = list,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = list.Count
            });
        }
    }

    [Fact]
    public async Task GetAsync_DeveMarcarVacinaComoAtrasada()
    {
        // Arrange
        var pets = new InMemoryPetRepository();
        await pets.AddAsync(new Pet { Nome = "Thor", TutorId = 1 });
        var vacinas = new InMemoryVacinas();
        await vacinas.AddAsync(new Vacina
        {
            Nome = "V8",
            PetId = 1,
            DataAplicacao = DateTime.Today.AddYears(-1),
            DataProximaDose = DateTime.Today.AddDays(-5)
        });
        var sut = new VacinaService(vacinas, pets, new VacinaDomainService(), new InMemoryUnitOfWork());

        // Act
        var pagina = await sut.GetAsync(new VacinaQueryParameters { Atrasadas = true });

        // Assert
        Assert.Single(pagina.Items);
        Assert.True(pagina.Items[0].Atrasada);
    }
}
