using System.ComponentModel.DataAnnotations;
using VitalPetTech.Application.Hateoas;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Application.DTOs;

public class LeituraSensorRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "O ID do pet é obrigatório.")]
    public int IdPet { get; set; }

    [EnumDataType(typeof(TipoAlerta), ErrorMessage = "Tipo de leitura inválido.")]
    public TipoAlerta Tipo { get; set; }

    public double? Valor { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90.")]
    public double? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180.")]
    public double? Longitude { get; set; }

    [Required(ErrorMessage = "A data e hora da leitura é obrigatória.")]
    public DateTime? Timestamp { get; set; }

    // Regras que dependem do tipo da leitura
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Tipo is TipoAlerta.Batimento or TipoAlerta.Temperatura && Valor is null)
            yield return new ValidationResult("Leituras de batimento e temperatura exigem o campo valor.", new[] { nameof(Valor) });

        if (Tipo == TipoAlerta.Localizacao && (Latitude is null || Longitude is null))
            yield return new ValidationResult("Leituras de localização exigem latitude e longitude.", new[] { nameof(Latitude), nameof(Longitude) });
    }
}

public record LeituraSensorResponse(
    string Id,
    int IdPet,
    TipoAlerta Tipo,
    double? Valor,
    double? Latitude,
    double? Longitude,
    DateTime Timestamp) : LinkedResource;