using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence.Repositories;

public abstract class EfRepository<T>(LifePetDbContext dbContext) : IRepository<T> where T : class
{
    protected LifePetDbContext Db => dbContext;
    protected DbSet<T> Set => dbContext.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await Set.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Set.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await Set.AddAsync(entity, cancellationToken);

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        => await Set.FindAsync([id], cancellationToken) is not null;
}
