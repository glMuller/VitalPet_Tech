using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Tests.Integration.Helpers;

public static class ApiTestHelper
{
    // A API devolve enums como texto, então o cliente precisa do mesmo conversor
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<TutorResponse> CriarTutorAsync(HttpClient client)
    {
        var request = new TutorRequest
        {
            NomeTutor = "Maria Souza",
            CpfTutor = "987.654.321-00",
            TelefoneTutor = "(11) 98888-0000",
            EnderecoTutor = "Av. Central, 456"
        };
        var resposta = await client.PostAsJsonAsync("/api/Tutor", request, JsonOptions);
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<TutorResponse>(JsonOptions))!;
    }

    public static async Task<PetResponse> CriarPetAsync(
        HttpClient client, int? tutorId = null, string especie = "Cachorro")
    {
        var idTutor = tutorId ?? (await CriarTutorAsync(client)).Id;
        var request = new PetRequest
        {
            Nome = "Thor",
            DataNascimento = new DateTime(2021, 5, 10),
            PesoPet = 20,
            EspeciePet = especie,
            IdTutor = idTutor
        };
        var resposta = await client.PostAsJsonAsync("/api/Pet", request, JsonOptions);
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<PetResponse>(JsonOptions))!;
    }

    public static async Task<AlertaResponse> CriarAlertaAsync(HttpClient client, int? petId = null)
    {
        var idPet = petId ?? (await CriarPetAsync(client)).Id;
        var request = new AlertaRequest
        {
            TipoAlerta = TipoAlerta.Batimento,
            Valor = 130,
            Severidade = Severidade.Critico,
            Timestamp = new DateTime(2026, 10, 1, 12, 0, 0),
            IdPet = idPet
        };
        var resposta = await client.PostAsJsonAsync("/api/Alerta", request, JsonOptions);
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<AlertaResponse>(JsonOptions))!;
    }
}