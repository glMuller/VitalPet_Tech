using System.Net.Http.Json;
using System.Text.Json;
using VitalPetTech.Tests.Integration.Fixtures;
using VitalPetTech.Tests.Integration.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class HateoasIntegrationTests
{
    private readonly HttpClient _client;

    public HateoasIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static List<string> Rels(JsonElement json) =>
        json.GetProperty("links").EnumerateArray()
            .Select(l => l.GetProperty("rel").GetString()!)
            .ToList();

    [Fact]
    public async Task GetById_PetExistente_RetornaLinksDeSelfTutorEAlertas()
    {
        // Arrange
        var pet = await ApiTestHelper.CriarPetAsync(_client);

        // Act
        var json = await _client.GetFromJsonAsync<JsonElement>($"/api/Pet/{pet.Id}");

        // Assert
        var rels = Rels(json);
        Assert.Contains("self", rels);
        Assert.Contains("tutor", rels);
        Assert.Contains("alertas", rels);
    }

    [Fact]
    public async Task GetAll_ComPaginaIntermediaria_RetornaLinksDeNavegacaoMantendoFiltro()
    {
        // Arrange
        var especie = $"Esp-{Guid.NewGuid():N}"[..20];
        var tutor = await ApiTestHelper.CriarTutorAsync(_client);
        await ApiTestHelper.CriarPetAsync(_client, tutor.Id, especie);
        await ApiTestHelper.CriarPetAsync(_client, tutor.Id, especie);

        // Act
        var json = await _client.GetFromJsonAsync<JsonElement>($"/api/Pet?especie={especie}&pageSize=1");

        // Assert
        var links = json.GetProperty("links").EnumerateArray().ToList();
        var next = links.Single(l => l.GetProperty("rel").GetString() == "next");
        var href = next.GetProperty("href").GetString()!;
        Assert.Contains("page=2", href);
        Assert.Contains($"especie={especie}", href);
    }

    [Fact]
    public async Task GetById_AlertaNaoResolvido_OferecaLinkParaResolver()
    {
        // Arrange
        var alerta = await ApiTestHelper.CriarAlertaAsync(_client);

        // Act
        var json = await _client.GetFromJsonAsync<JsonElement>($"/api/Alerta/{alerta.Id}");

        // Assert
        Assert.Contains("resolver", Rels(json));
    }
}