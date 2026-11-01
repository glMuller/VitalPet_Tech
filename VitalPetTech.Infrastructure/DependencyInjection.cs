using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Auth;
using VitalPetTech.Infrastructure.Data;
using VitalPetTech.Infrastructure.Mongo;
using VitalPetTech.Infrastructure.Repositories;

namespace VitalPetTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString, JwtSettings jwt, MongoSettings mongo)
    {
        // Oracle (relacional)
        services.AddDbContext<ApplicationContext>(options => options.UseOracle(connectionString));

        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IEventoClinicoRepository, EventoClinicoRepository>();
        services.AddScoped<IAlertaRepository, AlertaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // MongoDB (NoSQL): o cliente é thread-safe e deve ser único
        MongoMapping.Register();
        services.AddSingleton(mongo);
        services.AddSingleton<IMongoClient>(_ =>
        {
            var settings = MongoClientSettings.FromConnectionString(mongo.ConnectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            return new MongoClient(settings);
        });
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongo.DatabaseName));
        services.AddSingleton<ILeituraSensorRepository, LeituraSensorRepository>();

        // Segurança
        services.AddSingleton(jwt);
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}