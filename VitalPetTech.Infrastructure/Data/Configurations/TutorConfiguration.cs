using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Infrastructure.Data.Configurations;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("Tutor");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id_tutor").ValueGeneratedOnAdd();

        builder.Property(t => t.NomeTutor).HasColumnName("nome_tutor").HasMaxLength(200).IsRequired();
        builder.Property(t => t.CpfTutor).HasColumnName("cpf_tutor").HasMaxLength(14).IsRequired();
        builder.Property(t => t.TelefoneTutor).HasColumnName("telefone_tutor").HasMaxLength(15).IsRequired();
        builder.Property(t => t.EnderecoTutor).HasColumnName("endereco_tutor").HasMaxLength(200).IsRequired();
    }
}