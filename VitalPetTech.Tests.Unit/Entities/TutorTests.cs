using System.ComponentModel.DataAnnotations;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Entities;

public class TutorTests
{
    private static IList<ValidationResult> ValidarModelo(Tutor tutor)
    {
        var contexto = new ValidationContext(tutor);
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(tutor, contexto, resultados, validateAllProperties: true);
        return resultados;
    }

    [Fact]
    public void ValidarModelo_TutorComDadosValidos_NaoRetornaErros()
    {
        // Arrange
        var tutor = new Tutor
        {
            NomeTutor = "João da Silva",
            CpfTutor = "123.456.789-00",
            TelefoneTutor = "(11) 99999-0000",
            EnderecoTutor = "Rua Exemplo, 123"
        };

        // Act
        var resultados = ValidarModelo(tutor);

        // Assert
        Assert.Empty(resultados);
    }

    [Fact]
    public void ValidarModelo_NomeTutorVazio_RetornaErroDeValidacao()
    {
        // Arrange
        var tutor = new Tutor
        {
            NomeTutor = "",
            CpfTutor = "123.456.789-00",
            TelefoneTutor = "(11) 99999-0000",
            EnderecoTutor = "Rua Exemplo, 123"
        };

        // Act
        var resultados = ValidarModelo(tutor);

        // Assert
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Tutor.NomeTutor)));
    }

    [Fact]
    public void ValidarModelo_CpfComMaisDe14Caracteres_RetornaErroDeValidacao()
    {
        // Arrange
        var tutor = new Tutor
        {
            NomeTutor = "João da Silva",
            CpfTutor = "123.456.789-000000", // acima do limite de 14 caracteres
            TelefoneTutor = "(11) 99999-0000",
            EnderecoTutor = "Rua Exemplo, 123"
        };

        // Act
        var resultados = ValidarModelo(tutor);

        // Assert
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Tutor.CpfTutor)));
    }

    [Fact]
    public void ValidarModelo_EnderecoVazio_RetornaErroDeValidacao()
    {
        // Arrange
        var tutor = new Tutor
        {
            NomeTutor = "João da Silva",
            CpfTutor = "123.456.789-00",
            TelefoneTutor = "(11) 99999-0000",
            EnderecoTutor = ""
        };

        // Act
        var resultados = ValidarModelo(tutor);

        // Assert
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Tutor.EnderecoTutor)));
    }
}