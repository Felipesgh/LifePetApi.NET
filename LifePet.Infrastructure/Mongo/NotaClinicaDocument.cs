using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LifePet.Infrastructure.Mongo;

internal sealed class NotaClinicaDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    public int PetId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
