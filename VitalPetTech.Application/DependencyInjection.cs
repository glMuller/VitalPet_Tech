using Microsoft.Extensions.DependencyInjection;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Application.Services;

namespace VitalPetTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITutorService, TutorService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IEventoClinicoService, EventoClinicoService>();
        services.AddScoped<IAlertaService, AlertaService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}