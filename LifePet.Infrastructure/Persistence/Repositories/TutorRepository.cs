using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence.Repositories;

public class TutorRepository(LifePetDbContext db) : EfRepository<Tutor>(db), ITutorRepository
{
    public async Task<Tutor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await Set.AsNoTracking().FirstOrDefaultAsync(t => t.Email == email, cancellationToken);

    public Task<PagedResult<Tutor>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var source = Set.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(t => t.Nome.ToLower().Contains(term) || t.Email.ToLower().Contains(term));
        }

        source = query.SortBy?.ToLower() switch
        {
            "email" => query.SortDirection == "desc" ? source.OrderByDescending(t => t.Email) : source.OrderBy(t => t.Email),
            "telefone" => query.SortDirection == "desc" ? source.OrderByDescending(t => t.Telefone) : source.OrderBy(t => t.Telefone),
            "id" => query.SortDirection == "desc" ? source.OrderByDescending(t => t.Id) : source.OrderBy(t => t.Id),
            _ => query.SortDirection == "desc" ? source.OrderByDescending(t => t.Nome) : source.OrderBy(t => t.Nome)
        };

        return source.ToPagedAsync(query, cancellationToken);
    }
}
