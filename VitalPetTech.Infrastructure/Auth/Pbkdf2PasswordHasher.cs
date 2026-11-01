using System.Security.Cryptography;
using VitalPetTech.Application.Interfaces;

namespace VitalPetTech.Infrastructure.Auth;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 210_000;

    // Formato salvo: iterações.salt.hash (tudo em Base64)
    public string GerarHash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var chave = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(chave)}";
    }

    public bool Verificar(string senha, string hash)
    {
        var partes = hash.Split('.');
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes) || iteracoes <= 0)
            return false;

        try
        {
            var salt = Convert.FromBase64String(partes[1]);
            var esperado = Convert.FromBase64String(partes[2]);
            var atual = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(atual, esperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}