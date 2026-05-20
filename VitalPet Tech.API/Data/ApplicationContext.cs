using VitalPet_Tech.Entites;
using Microsoft.EntityFrameworkCore;

namespace VitalPet_Tech.API.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) 
        : base(options)
    {
    }

    
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Tutor>  Tutor { get; set; }
    public DbSet<EventoClinico> EventoClinico { get; set; }
    public DbSet<Alerta> Alerta { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alerta>()
            .Property(a => a.Resolvido)
            .HasColumnType("NUMBER(1)");
    }
}