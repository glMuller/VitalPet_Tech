using System.ComponentModel.DataAnnotations;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Entities;

public class AlertaTests
{
    private static IList<ValidationResult> ValidarModelo(Alerta alerta)
    {
        var contexto = new ValidationContext(alerta);
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(alerta, contexto, resultados, validateAllProperties: true);
        return resultados;
    }

    [Fact]
    public void ValidarModelo_AlertaComDadosValidos_NaoRetornaErros()
    {
        // Arrange
        var alerta = new Alerta
        {
            TipoAlerta = TipoAlerta.Batimento,
            Valor = 120.5,
            Severidade = Severidade.Atencao,
            Timestamp = DateTime.Now,
            IdPet = 1
        };

        // Act
        var resultados = ValidarModelo(alerta);

        // Assert
        Assert.Empty(resultados);
    }

    [Fact]
    public void ValidarModelo_NovoAlerta_NasceComoNaoResolvido()
    {
        // Arrange & Act
        var alerta = new Alerta
        {
            TipoAlerta = TipoAlerta.Temperatura,
            Valor = 39.5,
            Severidade = Severidade.Critico,
            Timestamp = DateTime.Now,
            IdPet = 1
        };

        // Assert
        Assert.False(alerta.Resolvido);
    }

    [Fact]
    public void ValidarModelo_AlertaDeLocalizacaoComCoordenadas_NaoRetornaErros()
    {
        // Arrange
        var alerta = new Alerta
        {
            TipoAlerta = TipoAlerta.Localizacao,
            Latitude = -23.5505,
            Longitude = -46.6333,
            Severidade = Severidade.Info,
            Timestamp = DateTime.Now,
            IdPet = 1
        };

        // Act
        var resultados = ValidarModelo(alerta);

        // Assert
        Assert.Empty(resultados);
    }
}