using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Data;
using VitalPetTech.Infrastructure.Repositories;

namespace VitalPetTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationContext>(options => options.UseOracle(connectionString));

        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IEventoClinicoRepository, EventoClinicoRepository>();
        services.AddScoped<IAlertaRepository, AlertaRepository>();

        return services;
    }
}