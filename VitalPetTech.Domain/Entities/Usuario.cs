namespace VitalPetTech.Domain.Entities;

public enum PerfilUsuario
{
    Usuario = 1,
    Admin = 2
}

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Usuario;
    public DateTime CriadoEm { get; set; }
}