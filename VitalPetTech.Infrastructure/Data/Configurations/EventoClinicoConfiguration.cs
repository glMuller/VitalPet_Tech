using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Infrastructure.Data.Configurations;

public class EventoClinicoConfiguration : IEntityTypeConfiguration<EventoClinico>
{
    public void Configure(EntityTypeBuilder<EventoClinico> builder)
    {
        builder.ToTable("Evento_Clinico");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id_evento").ValueGeneratedOnAdd();

        builder.Property(e => e.Tipo).HasColumnName("tipo_evento");
        builder.Property(e => e.DataEvento).HasColumnName("data_evento");
        builder.Property(e => e.Clinica).HasColumnName("clinica").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(500).IsRequired();
        builder.Property(e => e.ProximoRetorno).HasColumnName("proximo_retorno");
        builder.Property(e => e.IdPet).HasColumnName("id_pet");

        builder.HasOne(e => e.Pet)
            .WithMany()
            .HasForeignKey(e => e.IdPet)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}