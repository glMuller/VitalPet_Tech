namespace VitalPetTech.Application.Interfaces;

public interface IPasswordHasher
{
    string GerarHash(string senha);
    bool Verificar(string senha, string hash);
}