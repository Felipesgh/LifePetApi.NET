namespace LifePet.Infrastructure.Mongo;

public sealed class MongoSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "lifepet";
    public string NotasCollection { get; set; } = "notas_clinicas";
}
