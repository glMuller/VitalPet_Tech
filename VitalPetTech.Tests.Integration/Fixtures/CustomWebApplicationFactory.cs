using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Tests.Integration.Fixtures;

// Sobe a API inteira em memória, trocando o Oracle real por um banco InMemory
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:OracleConnection",
            "Data Source=localhost:1521/FAKE;User Id=teste;Password=teste;");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationContext>(options =>
                options.UseInMemoryDatabase("VitalPetTestDb"));
        });
    }
}

// Todos os testes de integração compartilham a mesma factory (a API sobe uma única vez)
[CollectionDefinition("API Integration Tests")]
public class ApiCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}