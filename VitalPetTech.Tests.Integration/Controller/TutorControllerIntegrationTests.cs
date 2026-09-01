using System.Net;
using System.Net.Http.Json;
using VitalPet_Tech.Entites;
using VitalPetTech.Tests.Integration.Fixtures;
using Xunit;

namespace VitalPetTech.Tests.Integration.Controllers;

[Collection("API Integration Tests")]
public class TutorControllerIntegrationTests
{
    private readonly HttpClient _client;

    public TutorControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_QuandoChamado_RetornaStatusOk()
    {
        // Arrange
        // (nenhuma preparação extra necessária)

        // Act
        var resposta = await _client.GetAsync("/api/Tutor");

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Create_TutorValido_RetornaCreatedComLocationHeader()
    {
        // Arrange
        var novoTutor = new Tutor
        {
            NomeTutor = "Maria Souza",
            CpfTutor = "987.654.321-00",
            TelefoneTutor = "(11) 98888-0000",
            EnderecoTutor = "Av. Central, 456"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Tutor", novoTutor);

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
    }

    [Fact]
    public async Task Create_TutorSemNome_RetornaBadRequest()
    {
        // Arrange
        var tutorInvalido = new Tutor
        {
            NomeTutor = "", // viola [Required]
            CpfTutor = "987.654.321-00",
            TelefoneTutor = "(11) 98888-0000",
            EnderecoTutor = "Av. Central, 456"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Tutor", tutorInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetById_TutorInexistente_RetornaNotFound()
    {
        // Arrange
        var idInexistente = 999999;

        // Act
        var resposta = await _client.GetAsync($"/api/Tutor/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }
}