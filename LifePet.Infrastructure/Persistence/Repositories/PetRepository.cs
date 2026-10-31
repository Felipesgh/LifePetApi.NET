using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence.Repositories;

public class PetRepository(LifePetDbContext db) : EfRepository<Pet>(db), IPetRepository
{
    public Task<PagedResult<Pet>> GetPagedAsync(PagedQuery query, int? tutorId, string? especie, CancellationToken cancellationToken = default)
    {
        var source = Set.AsNoTracking().AsQueryable();

        if (tutorId.HasValue)
            source = source.Where(p => p.TutorId == tutorId.Value);

        if (!string.IsNullOrWhiteSpace(especie))
            source = source.Where(p => p.Especie == especie);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(p => p.Nome.ToLower().Contains(term) || p.Raca.ToLower().Contains(term));
        }

        source = query.SortBy?.ToLower() switch
        {
            "especie" => query.SortDirection == "desc" ? source.OrderByDescending(p => p.Especie) : source.OrderBy(p => p.Especie),
            "datanascimento" => query.SortDirection == "desc" ? source.OrderByDescending(p => p.DataNascimento) : source.OrderBy(p => p.DataNascimento),
            "id" => query.SortDirection == "desc" ? source.OrderByDescending(p => p.Id) : source.OrderBy(p => p.Id),
            _ => query.SortDirection == "desc" ? source.OrderByDescending(p => p.Nome) : source.OrderBy(p => p.Nome)
        };

        return source.ToPagedAsync(query, cancellationToken);
    }
}
