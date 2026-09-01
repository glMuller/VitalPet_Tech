using System.ComponentModel.DataAnnotations;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Entities;

public class EventoClinicoTests
{
    private static IList<ValidationResult> ValidarModelo(EventoClinico evento)
    {
        var contexto = new ValidationContext(evento);
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(evento, contexto, resultados, validateAllProperties: true);
        return resultados;
    }

    [Fact]
    public void ValidarModelo_EventoComDadosValidos_NaoRetornaErros()
    {
        // Arrange
        var evento = new EventoClinico
        {
            Tipo = TipoEvento.Consulta,
            DataEvento = DateTime.Now,
            Clinica = "Clínica VitalPet",
            Descricao = "Consulta de rotina",
            IdPet = 1
        };

        // Act
        var resultados = ValidarModelo(evento);

        // Assert
        Assert.Empty(resultados);
    }

    [Fact]
    public void ValidarModelo_ClinicaVazia_RetornaErroDeValidacao()
    {
        // Arrange
        var evento = new EventoClinico
        {
            Tipo = TipoEvento.Vacina,
            DataEvento = DateTime.Now,
            Clinica = "",
            Descricao = "Vacina antirrábica",
            IdPet = 1
        };

        // Act
        var resultados = ValidarModelo(evento);

        // Assert
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(EventoClinico.Clinica)));
    }

    [Fact]
    public void ValidarModelo_DescricaoVazia_RetornaErroDeValidacao()
    {
        // Arrange
        var evento = new EventoClinico
        {
            Tipo = TipoEvento.Exame,
            DataEvento = DateTime.Now,
            Clinica = "Clínica VitalPet",
            Descricao = "",
            IdPet = 1
        };

        // Act
        var resultados = ValidarModelo(evento);

        // Assert
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(EventoClinico.Descricao)));
    }

    [Fact]
    public void ValidarModelo_ProximoRetornoNulo_NaoRetornaErro()
    {
        // Arrange — ProximoRetorno é opcional, então deve ser válido mesmo nulo
        var evento = new EventoClinico
        {
            Tipo = TipoEvento.Medicamento,
            DataEvento = DateTime.Now,
            Clinica = "Clínica VitalPet",
            Descricao = "Aplicação de medicamento",
            ProximoRetorno = null,
            IdPet = 1
        };

        // Act
        var resultados = ValidarModelo(evento);

        // Assert
        Assert.Empty(resultados);
    }
}