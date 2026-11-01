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
public class LeituraSensorControllerIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public LeituraSensorControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateAuthenticatedClient();
    }

    private static LeituraSensorRequest CriarRequest(int idPet) => new()
    {
        IdPet = idPet,
        Tipo = TipoAlerta.Batimento,
        Valor = 95,
        Timestamp = new DateTime(2026, 10, 1, 12, 0, 0)
    };

    [Fact]
    public async Task Create_PetExistente_RetornaCreatedComLocationHeader()
    {
        // Arrange
        var pet = await ApiTestHelper.CriarPetAsync(_client);

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/LeituraSensor", CriarRequest(pet.Id), ApiTestHelper.JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
    }

    [Fact]
    public async Task Create_PetInexistente_RetornaUnprocessableEntity()
    {
        // Arrange
        var request = CriarRequest(999999);

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/LeituraSensor", request, ApiTestHelper.JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_BatimentoSemValor_RetornaBadRequest()
    {
        // Arrange
        var request = CriarRequest(1);
        request.Valor = null;

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/LeituraSensor", request, ApiTestHelper.JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetAll_FiltroPorPet_RetornaApenasLeiturasDoPet()
    {
        // Arrange
        var pet = await ApiTestHelper.CriarPetAsync(_client);
        var outroPet = await ApiTestHelper.CriarPetAsync(_client);
        await _client.PostAsJsonAsync("/api/LeituraSensor", CriarRequest(pet.Id), ApiTestHelper.JsonOptions);
        await _client.PostAsJsonAsync("/api/LeituraSensor", CriarRequest(outroPet.Id), ApiTestHelper.JsonOptions);

        // Act
        var resposta = await _client.GetAsync($"/api/LeituraSensor?petId={pet.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        var itens = json.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(itens);
        Assert.Equal(pet.Id, itens[0].GetProperty("idPet").GetInt32());
    }

    [Fact]
    public async Task GetById_LeituraInexistente_RetornaNotFound()
    {
        // Arrange
        var idInexistente = "000000000000000000000000";

        // Act
        var resposta = await _client.GetAsync($"/api/LeituraSensor/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task GetAll_SemToken_RetornaUnauthorized()
    {
        // Arrange
        var clienteAnonimo = _factory.CreateClient();

        // Act
        var resposta = await clienteAnonimo.GetAsync("/api/LeituraSensor");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }
}