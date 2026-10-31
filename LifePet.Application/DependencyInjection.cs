using LifePet.Application.Services;
using LifePet.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LifePet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IVacinaDomainService, VacinaDomainService>();
        services.AddSingleton<IMedicamentoDomainService, MedicamentoDomainService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITutorService, TutorService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IVacinaService, VacinaService>();
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IMedicamentoService, MedicamentoService>();
        services.AddScoped<IHistoricoService, HistoricoService>();
        services.AddScoped<INotaClinicaService, NotaClinicaService>();
        return services;
    }
}
