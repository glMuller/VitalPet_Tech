using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VitalPet_Tech.API.Data;

namespace VitalPetTech.Tests.Integration.Fixtures;

// Sobe a API inteira em memória, mas troca o Oracle real por um banco InMemory
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext real (configurado com Oracle) que veio do Program.cs
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Registra um DbContext InMemory no lugar
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseInMemoryDatabase("VitalPetTestDb");
            });
        });
    }
}

// Define uma "coleção" de testes que compartilham a mesma factory (sobe a API uma única vez)
[CollectionDefinition("API Integration Tests")]
public class ApiCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}