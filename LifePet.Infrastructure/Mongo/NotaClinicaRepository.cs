using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LifePet.Infrastructure.Mongo;

public class NotaClinicaRepository : INotaClinicaRepository
{
    private readonly IMongoCollection<NotaClinicaDocument> _collection;

    public NotaClinicaRepository(IMongoClient client, IOptions<MongoSettings> options)
    {
        var settings = options.Value;
        _collection = client.GetDatabase(settings.Database).GetCollection<NotaClinicaDocument>(settings.NotasCollection);
    }

    public async Task<NotaClinica?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is null ? null : Map(document);
    }

    public async Task<PagedResult<NotaClinica>> GetPagedAsync(PagedQuery query, int? petId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<NotaClinicaDocument>.Filter.Empty;

        if (petId.HasValue)
            filter &= Builders<NotaClinicaDocument>.Filter.Eq(x => x.PetId, petId.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            filter &= Builders<NotaClinicaDocument>.Filter.Or(
                Builders<NotaClinicaDocument>.Filter.Regex(x => x.Titulo, new MongoDB.Bson.BsonRegularExpression(query.Search, "i")),
                Builders<NotaClinicaDocument>.Filter.Regex(x => x.Conteudo, new MongoDB.Bson.BsonRegularExpression(query.Search, "i")));
        }

        var total = (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var documents = await _collection.Find(filter)
            .SortByDescending(x => x.CriadoEm)
            .Skip((query.Page - 1) * query.PageSize)
            .Limit(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<NotaClinica>
        {
            Items = documents.Select(Map).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total
        };
    }

    public Task AddAsync(NotaClinica nota, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(Map(nota), cancellationToken: cancellationToken);

    public Task UpdateAsync(NotaClinica nota, CancellationToken cancellationToken = default)
        => _collection.ReplaceOneAsync(x => x.Id == nota.Id, Map(nota), cancellationToken: cancellationToken);

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);

    private static NotaClinica Map(NotaClinicaDocument document) => new()
    {
        Id = document.Id,
        PetId = document.PetId,
        Titulo = document.Titulo,
        Conteudo = document.Conteudo,
        Autor = document.Autor,
        CriadoEm = document.CriadoEm
    };

    private static NotaClinicaDocument Map(NotaClinica nota) => new()
    {
        Id = nota.Id,
        PetId = nota.PetId,
        Titulo = nota.Titulo,
        Conteudo = nota.Conteudo,
        Autor = nota.Autor,
        CriadoEm = nota.CriadoEm
    };
}
