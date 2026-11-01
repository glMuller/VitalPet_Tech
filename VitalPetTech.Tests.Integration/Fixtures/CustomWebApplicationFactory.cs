using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Tests.Integration.Fixtures;

// Sobe a API inteira em memória, trocando o Oracle real por um banco InMemory
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Dictionary<PerfilUsuario, string> _tokens = new();
    private readonly object _lock = new();

    public CustomWebApplicationFactory()
    {
        // O Program.cs lê essas configurações logo no início, então elas precisam
        // existir antes do host subir. Variáveis de ambiente garantem isso.
        Environment.SetEnvironmentVariable("Jwt__Key", "chave-secreta-somente-para-testes-0123456789");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "VitalPetTech.Tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "VitalPetTech.Tests.Clients");
        Environment.SetEnvironmentVariable("ConnectionStrings__OracleConnection",
            "Data Source=localhost:1521/FAKE;User Id=teste;Password=teste;");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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

    // Cliente já autenticado (Admin por padrão)
    public HttpClient CreateAuthenticatedClient(PerfilUsuario perfil = PerfilUsuario.Admin)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ObterToken(perfil));
        return client;
    }

    // Cria o usuário de teste uma única vez por perfil e reaproveita o token
    private string ObterToken(PerfilUsuario perfil)
    {
        lock (_lock)
        {
            if (_tokens.TryGetValue(perfil, out var existente))
                return existente;

            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            var usuario = new Usuario
            {
                Nome = $"Teste {perfil}",
                Email = $"{perfil}@teste.com".ToLowerInvariant(),
                SenhaHash = hasher.GerarHash("Senha@12345"),
                Perfil = perfil,
                CriadoEm = DateTime.UtcNow
            };
            context.Usuarios.Add(usuario);
            context.SaveChanges();

            var token = tokenService.Gerar(usuario).AccessToken;
            _tokens[perfil] = token;
            return token;
        }
    }
}

// Todos os testes de integração compartilham a mesma factory (a API sobe uma única vez)
[CollectionDefinition("API Integration Tests")]
public class ApiCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}