using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Infrastructure.Data.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id_pet").ValueGeneratedOnAdd();

        builder.Property(p => p.Nome).HasColumnName("nome_pet").HasMaxLength(100).IsRequired();
        builder.Property(p => p.DataNascimento).HasColumnName("data_nascimento");
        builder.Property(p => p.PesoPet).HasColumnName("peso_pet");
        builder.Property(p => p.EspeciePet).HasColumnName("especie_pet").HasMaxLength(50).IsRequired();
        builder.Property(p => p.IdTutor).HasColumnName("id_tutor");

        builder.HasOne(p => p.Tutor)
            .WithMany(t => t.Pets)
            .HasForeignKey(p => p.IdTutor)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}