using System.Net;
using System.Net.Http.Json;
using VitalPet_Tech.Entites;
using VitalPetTech.Tests.Integration.Fixtures;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class PetControllerIntegrationTests
{
    private readonly HttpClient _client;

    public PetControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_QuandoChamado_RetornaStatusOk()
    {
        // Arrange
        // (nenhuma preparação extra necessária além do client já configurado)

        // Act
        var resposta = await _client.GetAsync("/api/Pet");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_PetValido_RetornaCreatedComLocationHeader()
    {
        // Arrange
        var novoPet = new Pet
        {
            Nome = "Thor",
            EspeciePet = "Cachorro",
            PesoPet = 20,
            DataNascimento = new DateTime(2021, 5, 10),
            IdTutor = 1
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Pet", novoPet);

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
    }

    [Fact]
    public async Task Create_PetSemNome_RetornaBadRequest()
    {
        // Arrange
        var petInvalido = new Pet
        {
            Nome = "", // viola [Required]
            EspeciePet = "Gato",
            PesoPet = 5,
            DataNascimento = DateTime.Now,
            IdTutor = 1
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Pet", petInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
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
}