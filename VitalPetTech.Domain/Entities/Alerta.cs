namespace VitalPetTech.Domain.Entities;

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

public class Alerta
{
    public int Id { get; set; }
    public TipoAlerta TipoAlerta { get; set; }
    public double? Valor { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Severidade Severidade { get; set; }
    public bool Resolvido { get; set; } = false;
    public DateTime Timestamp { get; set; }

    public int IdPet { get; set; }
    public Pet? Pet { get; set; }
}