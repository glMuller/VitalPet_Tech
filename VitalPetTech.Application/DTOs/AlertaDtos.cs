using System.ComponentModel.DataAnnotations;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Application.DTOs;

public class AlertaRequest
{
    [EnumDataType(typeof(TipoAlerta), ErrorMessage = "Tipo de alerta inválido.")]
    public TipoAlerta TipoAlerta { get; set; }

    public double? Valor { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [EnumDataType(typeof(Severidade), ErrorMessage = "Severidade inválida.")]
    public Severidade Severidade { get; set; }

    [Required(ErrorMessage = "A data e hora do alerta é obrigatória.")]
    public DateTime? Timestamp { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "O ID do pet é obrigatório.")]
    public int IdPet { get; set; }
}

public class AlertaUpdateRequest : AlertaRequest
{
    public bool Resolvido { get; set; }
}

public record AlertaResponse(
    int Id,
    TipoAlerta TipoAlerta,
    double? Valor,
    double? Latitude,
    double? Longitude,
    Severidade Severidade,
    bool Resolvido,
    DateTime Timestamp,
    int IdPet,
    string NomePet);