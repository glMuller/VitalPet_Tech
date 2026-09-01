using System.Net;
using System.Net.Http.Json;
using VitalPet_Tech.Entites;
using VitalPetTech.Tests.Integration.Fixtures;
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

    [Fact]
    public async Task GetAll_QuandoChamado_RetornaStatusOk()
    {
        // Arrange
        // (nenhuma preparação extra necessária)

        // Act
        var resposta = await _client.GetAsync("/api/EventoClinico");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_EventoSemDescricao_RetornaBadRequest()
    {
        // Arrange
        var eventoInvalido = new EventoClinico
        {
            Tipo = TipoEvento.Consulta,
            DataEvento = DateTime.Now,
            Clinica = "Clínica VitalPet",
            Descricao = "", // viola [Required]
            IdPet = 1
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/EventoClinico", eventoInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
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