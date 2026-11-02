using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Interfaces;

namespace VitalPet_Tech.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>
    /// Registra um novo usuário. O primeiro usuário do sistema recebe o perfil Admin; os demais, Usuario.
    /// </summary>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UsuarioResponse>> Registrar(RegistrarRequest request)
    {
        var usuario = await _service.RegistrarAsync(request);
        return StatusCode(StatusCodes.Status201Created, usuario);
    }

    /// <summary>
    /// Autentica o usuário e devolve o token JWT para usar no header Authorization (Bearer).
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
        => Ok(await _service.LoginAsync(request));
}