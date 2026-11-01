using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Auth;
using VitalPetTech.Infrastructure.Data;
using VitalPetTech.Infrastructure.Repositories;

namespace VitalPetTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString, JwtSettings jwt)
    {
        services.AddDbContext<ApplicationContext>(options => options.UseOracle(connectionString));

        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IEventoClinicoRepository, EventoClinicoRepository>();
        services.AddScoped<IAlertaRepository, AlertaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        services.AddSingleton(jwt);
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}