using VitalPetTech.Application.DTOs;
using VitalPetTech.Tests.Unit.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Unit.Dtos;

public class TutorRequestTests
{
    private static TutorRequest CriarValido() => new()
    {
        NomeTutor = "João da Silva",
        CpfTutor = "123.456.789-00",
        TelefoneTutor = "(11) 99999-0000",
        EnderecoTutor = "Rua Exemplo, 123"
    };

    [Fact]
    public void Validar_TutorValido_NaoRetornaErros()
    {
        // Arrange
        var request = CriarValido();

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Empty(erros);
    }

    [Fact]
    public void Validar_NomeVazio_RetornaErroEmNomeTutor()
    {
        // Arrange
        var request = CriarValido();
        request.NomeTutor = "";

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(TutorRequest.NomeTutor)));
    }

    [Fact]
    public void Validar_CpfComMaisDe14Caracteres_RetornaErroEmCpfTutor()
    {
        // Arrange
        var request = CriarValido();
        request.CpfTutor = "123.456.789-000000";

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(TutorRequest.CpfTutor)));
    }

    [Fact]
    public void Validar_EnderecoVazio_RetornaErroEmEnderecoTutor()
    {
        // Arrange
        var request = CriarValido();
        request.EnderecoTutor = "";

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(TutorRequest.EnderecoTutor)));
    }
}