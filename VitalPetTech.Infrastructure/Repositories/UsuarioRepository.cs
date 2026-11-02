using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationContext _context;

    public UsuarioRepository(ApplicationContext context)
    {
        _context = context;
    }

    // O e-mail é sempre salvo em minúsculas pelo Service
    public Task<Usuario?> GetByEmailAsync(string email) =>
        _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public Task<bool> AnyAsync() => _context.Usuarios.AnyAsync();

    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }
}