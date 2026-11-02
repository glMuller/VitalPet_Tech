using System.ComponentModel.DataAnnotations;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Application.DTOs;

public class RegistrarRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome não pode ultrapassar 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(150, ErrorMessage = "O e-mail não pode ultrapassar 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
    [MaxLength(100, ErrorMessage = "A senha não pode ultrapassar 100 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}

public record UsuarioResponse(int Id, string Nome, string Email, PerfilUsuario Perfil);

public record TokenResponse(string AccessToken, string TokenType, DateTime ExpiraEm, PerfilUsuario Perfil);