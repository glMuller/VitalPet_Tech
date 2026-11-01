using System.Net;
using System.Net.Http.Json;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Tests.Integration.Fixtures;
using VitalPetTech.Tests.Integration.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class EventoClinicoControllerIntegrationTests
{
    private readonly HttpClient _client;

    public EventoClinicoControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static EventoClinicoRequest CriarRequest(int idPet, string descricao = "Consulta de rotina") => new()
    {
        Tipo = TipoEvento.Consulta,
        DataEvento = DateTime.Now,
        Clinica = "Clínica VitalPet",
        Descricao = descricao,
        IdPet = idPet
    };

    [Fact]
    public async Task GetAll_QuandoChamado_RetornaStatusOk()
    {
        // Arrange
        // (nenhuma preparação necessária)

        // Act
        var resposta = await _client.GetAsync("/api/EventoClinico");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_EventoValidoComPetExistente_RetornaCreated()
    {
        // Arrange
        var pet = await ApiTestHelper.CriarPetAsync(_client);

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/EventoClinico", CriarRequest(pet.Id));

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_EventoSemDescricao_RetornaBadRequest()
    {
        // Arrange
        var request = CriarRequest(idPet: 1, descricao: "");

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/EventoClinico", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_PetInexistente_RetornaUnprocessableEntity()
    {
        // Arrange
        var request = CriarRequest(idPet: 999999);

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/EventoClinico", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }

    [Fact]
    public async Task GetById_EventoInexistente_RetornaNotFound()
    {
        // Arrange
        var idInexistente = 999999;

        // Act
        var resposta = await _client.GetAsync($"/api/EventoClinico/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }
}