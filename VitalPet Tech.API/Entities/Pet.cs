using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalPet_Tech.Entites;


[Table("Pets")]
public class Pet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_pet")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome do pet é obrigatório")]
    [MaxLength(100, ErrorMessage = "O nome do pet não pode ultrapassar 100 caracteres")]
    [Column("nome_pet")]
    public string Nome { get; set; }
    
    [Required(ErrorMessage = "A data de nascimento do pet é obrigatória.")]
    [DataType(DataType.Date)]
    [Column("data_nascimento")]
    public DateTime DataNascimento { get; set; }
    
    [Required(ErrorMessage = "O peso do pet é obrigatório.")]
    [MaxLength(3,ErrorMessage = "O peso do pet não pode ultrapassar 3 caracteres (digite apenas números).")]
    [Column("peso_pet")]
    public string PesoPet { get; set; }
    
    [Required(ErrorMessage = "A espécie do animal é obrigatória.")]
    [MaxLength(50,ErrorMessage = "A espécie do animal não pode ultrapassar 50 caracteres")]
    [Column("especie_pet")]
    public string EspeciePet { get; set; }
    
    
}