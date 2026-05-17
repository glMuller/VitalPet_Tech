using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalPet_Tech.Entites;

public enum TipoEvento
{
    Consulta,
    Vacina,
    Exame,
    Cirurgia,
    Retorno,
    Medicamento
}

[Table("Evento_Clinico")]


public class EventoClinico
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_evento")]
    public int Id { get; set; }
    
    
    [Column("tipo_evento")]
    public TipoEvento Tipo { get; set; }

    [Required(ErrorMessage = "A data do evento é obrigatória.")]
    [DataType(DataType.Date)]
    [Column("data_evento")]
    public DateTime DataEvento { get; set; }

    [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome da clínica não pode ultrapassar 100 caracteres.")]
    [Column("clinica")]
    public string Clinica { get; set; }

    [Required(ErrorMessage = "A descrição do evento é obrigatória.")]
    [MaxLength(500, ErrorMessage = "A descrição não pode ultrapassar 500 caracteres.")]
    [Column("descricao")]
    public string Descricao { get; set; }

    //nem todo evento necessita de retorno
    [DataType(DataType.Date)]
    [Column("proximo_retorno")]
    public DateTime? ProximoRetorno { get; set; }
    
    [Required(ErrorMessage = "O ID do pet é obrigatório.")]
    [Column("id_pet")]
    public int IdPet { get; set; }

    [ForeignKey("IdPet")]
    public Pet? Pet { get; set; }
}