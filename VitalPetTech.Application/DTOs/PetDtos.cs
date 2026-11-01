using System.ComponentModel.DataAnnotations;

namespace VitalPetTech.Application.DTOs;

public class PetRequest
{
    [Required(ErrorMessage = "Nome do pet é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome do pet não pode ultrapassar 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento do pet é obrigatória.")]
    public DateTime? DataNascimento { get; set; }

    [Range(0.1, 100, ErrorMessage = "O peso do pet deve estar entre 0,1 e 100.")]
    public double PesoPet { get; set; }

    [Required(ErrorMessage = "A espécie do animal é obrigatória.")]
    [MaxLength(50, ErrorMessage = "A espécie do animal não pode ultrapassar 50 caracteres.")]
    public string EspeciePet { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "ID de tutor obrigatório.")]
    public int IdTutor { get; set; }
}

public record PetResponse(
    int Id,
    string Nome,
    DateTime DataNascimento,
    double PesoPet,
    string EspeciePet,
    int IdTutor,
    string NomeTutor);