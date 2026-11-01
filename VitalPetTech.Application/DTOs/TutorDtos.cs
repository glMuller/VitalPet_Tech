using System.ComponentModel.DataAnnotations;
using VitalPetTech.Application.Hateoas;

namespace VitalPetTech.Application.DTOs;

public class TutorRequest
{
    [Required(ErrorMessage = "Nome do tutor obrigatório.")]
    [MaxLength(200, ErrorMessage = "O nome do tutor deve ter no máximo 200 caracteres.")]
    public string NomeTutor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF do tutor é obrigatório.")]
    [MaxLength(14, ErrorMessage = "O CPF do tutor deve ter até 14 caracteres.")]
    public string CpfTutor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone do tutor é obrigatório.")]
    [MaxLength(15, ErrorMessage = "O telefone do tutor deve ter até 15 caracteres.")]
    public string TelefoneTutor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O endereço do tutor é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O endereço deve conter no máximo 200 caracteres.")]
    public string EnderecoTutor { get; set; } = string.Empty;
}

public record PetResumoResponse(int Id, string Nome, string EspeciePet);

public record TutorResponse(
    int Id,
    string NomeTutor,
    string CpfTutor,
    string TelefoneTutor,
    string EnderecoTutor,
    IReadOnlyList<PetResumoResponse> Pets) : LinkedResource;