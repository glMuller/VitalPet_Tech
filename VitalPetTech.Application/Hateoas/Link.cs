namespace VitalPetTech.Application.Hateoas;

public record Link(string Href, string Rel, string Method);

// Base dos responses que carregam links. Tem que ser "record" porque os DTOs são records
public abstract record LinkedResource
{
    public IReadOnlyList<Link> Links { get; init; } = Array.Empty<Link>();
}