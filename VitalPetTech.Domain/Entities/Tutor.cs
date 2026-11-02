namespace VitalPetTech.Domain.Entities;

public class Tutor
{
    public int Id { get; set; }
    public string NomeTutor { get; set; } = string.Empty;
    public string CpfTutor { get; set; } = string.Empty;
    public string TelefoneTutor { get; set; } = string.Empty;
    public string EnderecoTutor { get; set; } = string.Empty;

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}