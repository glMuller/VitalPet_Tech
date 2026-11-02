using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id_usuario").ValueGeneratedOnAdd();

        builder.Property(u => u.Nome).HasColumnName("nome_usuario").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        builder.Property(u => u.SenhaHash).HasColumnName("senha_hash").HasMaxLength(200).IsRequired();
        builder.Property(u => u.Perfil).HasColumnName("perfil");
        builder.Property(u => u.CriadoEm).HasColumnName("criado_em");

        // Garante no banco que não existem dois usuários com o mesmo e-mail
        builder.HasIndex(u => u.Email).IsUnique();
    }
    public DbSet<Usuario> Usuarios { get; set; } = null!;
}