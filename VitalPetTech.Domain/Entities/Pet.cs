namespace VitalPetTech.Domain.Entities;

public class Pet
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public double PesoPet { get; set; }
    public string EspeciePet { get; set; } = string.Empty;

    public int IdTutor { get; set; }
    public Tutor? Tutor { get; set; }
}