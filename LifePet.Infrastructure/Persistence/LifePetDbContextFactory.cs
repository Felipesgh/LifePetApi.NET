using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LifePet.Infrastructure.Persistence;

public sealed class LifePetDbContextFactory : IDesignTimeDbContextFactory<LifePetDbContext>
{
    public LifePetDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var options = new DbContextOptionsBuilder<LifePetDbContext>();
        options.UseOracle(
            configuration.GetConnectionString("OracleConnection"),
            oracle => oracle.MigrationsAssembly("LifePetApi"));
        return new LifePetDbContext(options.Options);
    }
}
