using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> AnyAsync();
    Task AddAsync(Usuario usuario);
}