using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace VitalPet_Tech.Entites;

public enum TipoAlerta
{
    Batimento = 1,
    Temperatura = 2,
    Localizacao = 3
}
public enum Severidade
{
    Info = 1,
    Atencao = 2,
    Critico = 3
}

[Table("Alerta")]
public class Alerta
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_alerta")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O tipo de alerta é obrigatório.")]
    [Column("tipo_alerta")]
    public TipoAlerta TipoAlerta { get; set; }
    
    [Column("valor")]
    public double? Valor { get; set; }
    
    [Column("latitude")]
    public double? Latitude { get; set; }
    
    [Column("longitude")]
    public double? Longitude { get; set; }
    
    [Column("severidade")]
    public Severidade Severidade { get; set; }

    [Column("resolvido")] public bool Resolvido { get; set; } = false;
    
    [Required(ErrorMessage = "A data e hora do alerta é obrigatório.")]
    [DataType(DataType.DateTime)]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
    
    
    [Required(ErrorMessage = "O ID do pet é obrigatório.")]
    [Column("id_pet")]
    public int IdPet { get; set; }
    [ForeignKey("IdPet")]
    public Pet? Pet { get; set; }
}