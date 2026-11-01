using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Tests.Integration.Fixtures;
using VitalPetTech.Tests.Integration.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class TutorControllerIntegrationTests
{
    private readonly HttpClient _client;

    public TutorControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task GetAll_ComPaginacao_RetornaOkComEstruturaPaginada()
    {
        // Arrange
        await ApiTestHelper.CriarTutorAsync(_client);

        // Act
        var resposta = await _client.GetAsync("/api/Tutor?page=1&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("items", out _));
        Assert.True(json.TryGetProperty("totalCount", out _));
    }

    [Fact]
    public async Task Create_TutorValido_RetornaCreatedComLocationHeader()
    {
        // Arrange
        var request = new TutorRequest
        {
            NomeTutor = "Maria Souza",
            CpfTutor = "987.654.321-00",
            TelefoneTutor = "(11) 98888-0000",
            EnderecoTutor = "Av. Central, 456"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Tutor", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
    }

    [Fact]
    public async Task Create_TutorSemNome_RetornaBadRequest()
    {
        // Arrange
        var request = new TutorRequest
        {
            NomeTutor = "",
            CpfTutor = "987.654.321-00",
            TelefoneTutor = "(11) 98888-0000",
            EnderecoTutor = "Av. Central, 456"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Tutor", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetById_TutorInexistente_RetornaNotFoundComProblemDetails()
    {
        // Arrange
        var idInexistente = 999999;

        // Act
        var resposta = await _client.GetAsync($"/api/Tutor/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(404, json.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task Delete_TutorExistente_RetornaNoContentEDepoisNotFound()
    {
        // Arrange
        var tutor = await ApiTestHelper.CriarTutorAsync(_client);

        // Act
        var delete = await _client.DeleteAsync($"/api/Tutor/{tutor.Id}");
        var consulta = await _client.GetAsync($"/api/Tutor/{tutor.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, consulta.StatusCode);
    }
}
