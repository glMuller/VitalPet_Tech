using System.ComponentModel.DataAnnotations;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Entities;

public class PetTests
{
    private static IList<ValidationResult> ValidarModelo(Pet pet)
    {
        var contexto = new ValidationContext(pet);
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(pet, contexto, resultados, validateAllProperties: true);
        return resultados;
    }

    [Fact]
    public void ValidarModelo_PetComDadosValidos_NaoRetornaErros()
    {
        // Arrange
        var pet = new Pet
        {
            Nome = "Rex",
            DataNascimento = new DateTime(2022, 1, 1),
            PesoPet = 12.5,
            EspeciePet = "Cachorro",
            IdTutor = 1
        };

        // Act
        var resultados = ValidarModelo(pet);

        // Assert
        Assert.Empty(resultados);
    }

    [Fact]
    public void ValidarModelo_PesoForaDoIntervalo_RetornaErroDeValidacao()
    {
        // Arrange
        var pet = new Pet
        {
            Nome = "Rex",
            DataNascimento = new DateTime(2022, 1, 1),
            PesoPet = 150,
            EspeciePet = "Cachorro",
            IdTutor = 1
        };

        // Act
        var resultados = ValidarModelo(pet);

        // Assert
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Pet.PesoPet)));
    }

    [Fact]
    public void ValidarModelo_NomeVazio_RetornaErroDeValidacao()
    {
        // Arrange
        var pet = new Pet
        {
            Nome = "",
            DataNascimento = new DateTime(2022, 1, 1),
            PesoPet = 10,
            EspeciePet = "Gato",
            IdTutor = 1
        };

        // Act
        var resultados = ValidarModelo(pet);

        // Assert
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Pet.Nome)));
    }
}