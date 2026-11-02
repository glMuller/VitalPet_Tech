using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Tests.Integration.Fixtures;
using VitalPetTech.Tests.Integration.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class PetControllerIntegrationTests
{
    private readonly HttpClient _client;

    public PetControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task GetAll_QuandoChamado_RetornaStatusOk()
    {
        // Arrange
        // (nenhuma preparaÃ§Ã£o necessÃ¡ria)

        // Act
        var resposta = await _client.GetAsync("/api/Pet");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_PetValidoComTutorExistente_RetornaCreatedComLocationHeader()
    {
        // Arrange
        var tutor = await ApiTestHelper.CriarTutorAsync(_client);
        var request = new PetRequest
        {
            Nome = "Thor",
            DataNascimento = new DateTime(2021, 5, 10),
            PesoPet = 20,
            EspeciePet = "Cachorro",
            IdTutor = tutor.Id
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Pet", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
    }

    [Fact]
    public async Task Create_PetSemNome_RetornaBadRequest()
    {
        // Arrange
        var request = new PetRequest
        {
            Nome = "",
            DataNascimento = DateTime.Now,
            PesoPet = 5,
            EspeciePet = "Gato",
            IdTutor = 1
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Pet", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_TutorInexistente_RetornaUnprocessableEntity()
    {
        // Arrange
        var request = new PetRequest
        {
            Nome = "Thor",
            DataNascimento = new DateTime(2021, 5, 10),
            PesoPet = 20,
            EspeciePet = "Cachorro",
            IdTutor = 999999
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Pet", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }

    [Fact]
    public async Task GetById_PetInexistente_RetornaNotFound()
    {
        // Arrange
        var idInexistente = 999999;

        // Act
        var resposta = await _client.GetAsync($"/api/Pet/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task GetAll_FiltroPorEspecie_RetornaApenasPetsDaEspecie()
    {
        // Arrange
        var especie = $"Esp-{Guid.NewGuid():N}"[..20];
        await ApiTestHelper.CriarPetAsync(_client, especie: especie);

        // Act
        var resposta = await _client.GetAsync($"/api/Pet?especie={especie}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        var itens = json.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(itens);
        Assert.Equal(especie, itens[0].GetProperty("especiePet").GetString());
    }
}
