using System.ComponentModel.DataAnnotations;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Application.DTOs;

public class EventoClinicoRequest
{
    [EnumDataType(typeof(TipoEvento), ErrorMessage = "Tipo de evento inválido.")]
    public TipoEvento Tipo { get; set; }

    [Required(ErrorMessage = "A data do evento é obrigatória.")]
    public DateTime? DataEvento { get; set; }

    [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome da clínica não pode ultrapassar 100 caracteres.")]
    public string Clinica { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição do evento é obrigatória.")]
    [MaxLength(500, ErrorMessage = "A descrição não pode ultrapassar 500 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    // Nem todo evento necessita de retorno
    public DateTime? ProximoRetorno { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "O ID do pet é obrigatório.")]
    public int IdPet { get; set; }
}

public record EventoClinicoResponse(
    int Id,
    TipoEvento Tipo,
    DateTime DataEvento,
    string Clinica,
    string Descricao,
    DateTime? ProximoRetorno,
    int IdPet,
    string NomePet);