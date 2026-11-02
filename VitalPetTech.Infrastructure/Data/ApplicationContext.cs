using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Infrastructure.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<Tutor> Tutores { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<EventoClinico> EventosClinicos { get; set; } = null!;
    public DbSet<Alerta> Alertas { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }
}