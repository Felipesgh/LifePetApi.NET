using LifePet.Infrastructure.Mongo;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LifePet.Infrastructure.HealthChecks;

public sealed class MongoConnectionHealthCheck(IMongoClient client, IOptions<MongoSettings> options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await client.GetDatabase(options.Value.Database).RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy("Conexão com o MongoDB disponível.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Falha ao verificar o MongoDB.", exception);
        }
    }
}
