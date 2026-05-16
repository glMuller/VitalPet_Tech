using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalPet_Tech.Entites;

[Table("Tutor")]

public class Tutor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_tutor")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome do tutor obrigatório.")]
    [MaxLength(200, ErrorMessage = " O nome do tutor deve ter no máximo 200 caracteres.")]
    [Column("nome_tutor")]
    public string NomeTutor { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O CPF do tutor é obrigatório.")]
    [MaxLength(14,ErrorMessage = "O CPF do tutor deve ter até 14 caracteres.")]
    [Column("cpf_tutor")]
    public string CpfTutor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone do tutor é obrigatório")]
    [MaxLength(15, ErrorMessage = "O telefone do tutor deve ter até 15 caracteres.")]
    [Column("telefone_tutor")]
    public string TelefoneTutor { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O endereço do tutor é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O endereço deve conter no máximo 200 caracteres.")]
    [Column("endereco_tutor")]
    public string EnderecoTutor { get; set; } = string.Empty;
}

