namespace VitalPetTech.Domain.Common;

public class TutorFilter : PagedQuery
{
    public string? Nome { get; set; }
}

public class PetFilter : PagedQuery
{
    public string? Especie { get; set; }
    public int? TutorId { get; set; }
}

public class EventoClinicoFilter : PagedQuery
{
    public int? PetId { get; set; }
    public Entities.TipoEvento? Tipo { get; set; }
}

public class AlertaFilter : PagedQuery
{
    public int? PetId { get; set; }
    public bool? Resolvido { get; set; }
    public Entities.Severidade? Severidade { get; set; }
    public Entities.TipoAlerta? TipoAlerta { get; set; }
}