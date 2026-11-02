using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Tests.Integration.Fixtures;
using VitalPetTech.Tests.Integration.Helpers;

namespace VitalPetTech.Tests.Integration.Controllers;

public class AuthControllerIntegrationTests
{
    [Fact]
    public async Task RegistrarELogin_ValidamBootstrapCredenciaisEAutorizacao()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = new RegistrarRequest
        {
            Nome = "Administrador de teste", Email = "ADMIN@teste.com", Senha = "Senha@12345"
        };

        var registro = await client.PostAsJsonAsync("/api/Auth/registrar", request);
        Assert.Equal(HttpStatusCode.Created, registro.StatusCode);
        var admin = await registro.Content.ReadFromJsonAsync<UsuarioResponse>(ApiTestHelper.JsonOptions);
        Assert.Equal(PerfilUsuario.Admin, admin!.Perfil);
        Assert.Equal("admin@teste.com", admin.Email);

        var duplicado = await client.PostAsJsonAsync("/api/Auth/registrar", request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, duplicado.StatusCode);

        request.Email = "usuario@teste.com";
        var segundoRegistro = await client.PostAsJsonAsync("/api/Auth/registrar", request);
        Assert.Equal(HttpStatusCode.Created, segundoRegistro.StatusCode);
        var usuario = await segundoRegistro.Content.ReadFromJsonAsync<UsuarioResponse>(ApiTestHelper.JsonOptions);
        Assert.Equal(PerfilUsuario.Usuario, usuario!.Perfil);

        var login = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = admin.Email, Senha = request.Senha });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var token = await login.Content.ReadFromJsonAsync<TokenResponse>(ApiTestHelper.JsonOptions);
        Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/Tutor")).StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/Tutor")).StatusCode);

        var senhaErrada = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = admin.Email, Senha = "senha-incorreta" });
        Assert.Equal(HttpStatusCode.Unauthorized, senhaErrada.StatusCode);
        request.Senha = "curta";
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PostAsJsonAsync("/api/Auth/registrar", request)).StatusCode);
    }
}
