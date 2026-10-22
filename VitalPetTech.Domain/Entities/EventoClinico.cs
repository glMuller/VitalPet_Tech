namespace VitalPetTech.Domain.Entities;

public enum TipoEvento
{
    Consulta,
    Vacina,
    Exame,
    Cirurgia,
    Retorno,
    Medicamento
}

public class EventoClinico
{
    public int Id { get; set; }
    public TipoEvento Tipo { get; set; }
    public DateTime DataEvento { get; set; }
    public string Clinica { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime? ProximoRetorno { get; set; }

    public int IdPet { get; set; }
    public Pet? Pet { get; set; }
}