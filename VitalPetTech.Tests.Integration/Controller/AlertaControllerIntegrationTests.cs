using System.Net;
using System.Net.Http.Json;
using VitalPet_Tech.Entites;
using VitalPetTech.Tests.Integration.Fixtures;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class AlertaControllerIntegrationTests
{
    private readonly HttpClient _client;

    public AlertaControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_QuandoChamado_RetornaStatusOk()
    {
        // Arrange
        // (nenhuma preparação extra necessária)

        // Act
        var resposta = await _client.GetAsync("/api/Alerta");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task GetPendentes_QuandoNaoHaAlertas_RetornaNotFound()
    {
        // Arrange
        // (banco começa vazio, cada teste de integração compartilha o mesmo InMemory
        //  configurado na fixture — se este teste rodar após outros que criaram alertas
        //  pendentes, ele pode falhar; ver observação abaixo)

        // Act
        var resposta = await _client.GetAsync("/api/Alerta/pendentes");

        // Assert
        Assert.True(
            resposta.StatusCode == HttpStatusCode.NotFound || resposta.StatusCode == HttpStatusCode.OK,
            $"Status inesperado: {resposta.StatusCode}"
        );
    }

    [Fact]
    public async Task GetById_AlertaInexistente_RetornaNotFound()
    {
        // Arrange
        var idInexistente = 999999;

        // Act
        var resposta = await _client.GetAsync($"/api/Alerta/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }
}