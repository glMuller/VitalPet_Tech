using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Infrastructure.Data.Configurations;

public class AlertaConfiguration : IEntityTypeConfiguration<Alerta>
{
    public void Configure(EntityTypeBuilder<Alerta> builder)
    {
        builder.ToTable("Alerta");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id_alerta").ValueGeneratedOnAdd();

        builder.Property(a => a.TipoAlerta).HasColumnName("tipo_alerta");
        builder.Property(a => a.Valor).HasColumnName("valor");
        builder.Property(a => a.Latitude).HasColumnName("latitude");
        builder.Property(a => a.Longitude).HasColumnName("longitude");
        builder.Property(a => a.Severidade).HasColumnName("severidade");
        builder.Property(a => a.Timestamp).HasColumnName("timestamp");
        builder.Property(a => a.IdPet).HasColumnName("id_pet");

        builder.Property(a => a.Resolvido)
            .HasColumnName("resolvido")
            .HasColumnType("NUMBER(1)")
            .HasConversion<int>();

        builder.HasOne(a => a.Pet)
            .WithMany()
            .HasForeignKey(a => a.IdPet)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}