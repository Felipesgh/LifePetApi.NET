using LifePet.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LifePet.Infrastructure.HealthChecks;

public sealed class OracleConnectionHealthCheck(LifePetDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Conexão com o Oracle disponível.")
                : HealthCheckResult.Unhealthy("Conexão com o Oracle indisponível.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Falha ao verificar o Oracle.", exception);
        }
    }
}
