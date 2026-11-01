using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Tests.Integration.Fixtures;
using VitalPetTech.Tests.Integration.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class AlertaControllerIntegrationTests
{
    private readonly HttpClient _client;

    public AlertaControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task GetAll_QuandoChamado_RetornaStatusOk()
    {
        // Arrange
        // (nenhuma preparaÃ§Ã£o necessÃ¡ria)

        // Act
        var resposta = await _client.GetAsync("/api/Alerta");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_AlertaValido_RetornaCreatedENasceNaoResolvido()
    {
        // Arrange
        var pet = await ApiTestHelper.CriarPetAsync(_client);
        var request = new AlertaRequest
        {
            TipoAlerta = TipoAlerta.Temperatura,
            Valor = 39.8,
            Severidade = Severidade.Atencao,
            Timestamp = DateTime.Now,
            IdPet = pet.Id
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Alerta", request, ApiTestHelper.JsonOptions);
        var criado = await resposta.Content.ReadFromJsonAsync<AlertaResponse>(ApiTestHelper.JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.False(criado!.Resolvido);
    }

    [Fact]
    public async Task Update_MarcarComoResolvido_RetornaNoContentEPersisteAlteracao()
    {
        // Arrange
        var alerta = await ApiTestHelper.CriarAlertaAsync(_client);
        var request = new AlertaUpdateRequest
        {
            TipoAlerta = alerta.TipoAlerta,
            Severidade = alerta.Severidade,
            Timestamp = alerta.Timestamp,
            IdPet = alerta.IdPet,
            Resolvido = true
        };

        // Act
        var put = await _client.PutAsJsonAsync($"/api/Alerta/{alerta.Id}", request, ApiTestHelper.JsonOptions);
        var consulta = await _client.GetFromJsonAsync<AlertaResponse>(
            $"/api/Alerta/{alerta.Id}", ApiTestHelper.JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, put.StatusCode);
        Assert.True(consulta!.Resolvido);
    }

    [Fact]
    public async Task GetAll_FiltroResolvidoTrue_RetornaSomenteAlertasResolvidos()
    {
        // Arrange
        var alerta = await ApiTestHelper.CriarAlertaAsync(_client);
        var request = new AlertaUpdateRequest
        {
            TipoAlerta = alerta.TipoAlerta,
            Severidade = alerta.Severidade,
            Timestamp = alerta.Timestamp,
            IdPet = alerta.IdPet,
            Resolvido = true
        };
        await _client.PutAsJsonAsync($"/api/Alerta/{alerta.Id}", request, ApiTestHelper.JsonOptions);

        // Act
        var resposta = await _client.GetAsync("/api/Alerta?resolvido=true&pageSize=50");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        var itens = json.GetProperty("items").EnumerateArray().ToList();
        Assert.NotEmpty(itens);
        Assert.All(itens, i => Assert.True(i.GetProperty("resolvido").GetBoolean()));
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
