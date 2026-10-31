using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;

namespace LifePet.Application.Tests.Fakes;

internal sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public int Saves { get; private set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saves++;
        return Task.FromResult(1);
    }
}

internal sealed class InMemoryTutorRepository : ITutorRepository
{
    private readonly List<Tutor> _items = [];
    private int _nextId = 1;

    public IReadOnlyList<Tutor> Items => _items;

    public Task<Tutor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task<IReadOnlyList<Tutor>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Tutor>>(_items);

    public Task AddAsync(Tutor entity, CancellationToken cancellationToken = default)
    {
        entity.Id = _nextId++;
        _items.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Tutor entity) { }

    public void Remove(Tutor entity) => _items.Remove(entity);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_items.Any(x => x.Id == id));

    public Task<Tutor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(_items.FirstOrDefault(x => x.Email == email));

    public Task<PagedResult<Tutor>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var filtered = _items.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(query.Search))
            filtered = filtered.Where(t => t.Nome.Contains(query.Search, StringComparison.OrdinalIgnoreCase));

        var list = filtered.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
        return Task.FromResult(new PagedResult<Tutor>
        {
            Items = list,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = _items.Count
        });
    }
}

internal sealed class InMemoryPetRepository : IPetRepository
{
    private readonly List<Pet> _items = [];
    private int _nextId = 1;

    public Task<Pet?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Pet>>(_items);

    public Task AddAsync(Pet entity, CancellationToken cancellationToken = default)
    {
        entity.Id = _nextId++;
        _items.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Pet entity) { }

    public void Remove(Pet entity) => _items.Remove(entity);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_items.Any(x => x.Id == id));

    public Task<PagedResult<Pet>> GetPagedAsync(PagedQuery query, int? tutorId, string? especie, CancellationToken cancellationToken = default)
    {
        var filtered = _items.AsEnumerable();
        if (tutorId.HasValue)
            filtered = filtered.Where(p => p.TutorId == tutorId.Value);
        if (!string.IsNullOrWhiteSpace(especie))
            filtered = filtered.Where(p => p.Especie == especie);

        var all = filtered.ToList();
        return Task.FromResult(new PagedResult<Pet>
        {
            Items = all.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = all.Count
        });
    }
}
