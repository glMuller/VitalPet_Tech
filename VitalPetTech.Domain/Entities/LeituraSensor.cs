namespace VitalPetTech.Domain.Entities;

public class LeituraSensor
{
    public string Id { get; set; } = string.Empty;
    public int IdPet { get; set; }
    public TipoAlerta Tipo { get; set; }
    public double? Valor { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}