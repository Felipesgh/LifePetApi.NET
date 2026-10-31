using System.Text;
using LifePet.Application.Abstractions;
using LifePet.Domain.Repositories;
using LifePet.Infrastructure.Auth;
using LifePet.Infrastructure.HealthChecks;
using LifePet.Infrastructure.Mongo;
using LifePet.Infrastructure.Persistence;
using LifePet.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace LifePet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LifePetDbContext>(options =>
            options.UseOracle(
                configuration.GetConnectionString("OracleConnection"),
                oracle => oracle.MigrationsAssembly("LifePetApi")));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<LifePetDbContext>());
        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IVacinaRepository, VacinaRepository>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
        services.AddScoped<IHistoricoRepository, HistoricoRepository>();
        services.AddScoped<INotaClinicaRepository, NotaClinicaRepository>();

        services.Configure<MongoSettings>(configuration.GetSection("Mongo"));
        services.AddSingleton<IMongoClient>(provider =>
        {
            var settings = configuration.GetSection("Mongo").Get<MongoSettings>() ?? new MongoSettings();
            return new MongoClient(settings.ConnectionString);
        });

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IAuthUserStore, ConfigAuthUserStore>();

        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                };
            });
        services.AddAuthorization();

        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("LifePet API está saudável."), tags: ["live"])
            .AddCheck<OracleConnectionHealthCheck>("oracle", tags: ["ready"])
            .AddCheck<MongoConnectionHealthCheck>("mongodb", tags: ["ready"]);

        return services;
    }
}
