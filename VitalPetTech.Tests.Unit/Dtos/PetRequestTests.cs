using VitalPetTech.Application.DTOs;
using VitalPetTech.Tests.Unit.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Unit.Dtos;

public class PetRequestTests
{
    private static PetRequest CriarValido() => new()
    {
        Nome = "Rex",
        DataNascimento = new DateTime(2022, 1, 1),
        PesoPet = 12.5,
        EspeciePet = "Cachorro",
        IdTutor = 1
    };

    [Fact]
    public void Validar_PetValido_NaoRetornaErros()
    {
        // Arrange
        var request = CriarValido();

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Empty(erros);
    }

    [Fact]
    public void Validar_PesoAcimaDoLimite_RetornaErroEmPesoPet()
    {
        // Arrange
        var request = CriarValido();
        request.PesoPet = 150;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(PetRequest.PesoPet)));
    }

    [Fact]
    public void Validar_DataNascimentoNula_RetornaErroEmDataNascimento()
    {
        // Arrange
        var request = CriarValido();
        request.DataNascimento = null;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(PetRequest.DataNascimento)));
    }

    [Fact]
    public void Validar_IdTutorZero_RetornaErroEmIdTutor()
    {
        // Arrange
        var request = CriarValido();
        request.IdTutor = 0;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(PetRequest.IdTutor)));
    }
}