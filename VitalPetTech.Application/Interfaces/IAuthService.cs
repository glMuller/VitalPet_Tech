using VitalPetTech.Application.DTOs;

namespace VitalPetTech.Application.Interfaces;

public interface IAuthService
{
    Task<UsuarioResponse> RegistrarAsync(RegistrarRequest request);
    Task<TokenResponse> LoginAsync(LoginRequest request);
}