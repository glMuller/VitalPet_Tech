using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Tests.Unit.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Unit.Dtos;

public class LeituraSensorRequestTests
{
    private static LeituraSensorRequest CriarValido() => new()
    {
        IdPet = 1,
        Tipo = TipoAlerta.Batimento,
        Valor = 90,
        Timestamp = DateTime.Now
    };

    [Fact]
    public void Validar_LeituraDeBatimentoValida_NaoRetornaErros()
    {
        // Arrange
        var request = CriarValido();

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Empty(erros);
    }

    [Fact]
    public void Validar_BatimentoSemValor_RetornaErroEmValor()
    {
        // Arrange
        var request = CriarValido();
        request.Valor = null;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(LeituraSensorRequest.Valor)));
    }

    [Fact]
    public void Validar_LocalizacaoSemCoordenadas_RetornaErroEmLatitude()
    {
        // Arrange
        var request = CriarValido();
        request.Tipo = TipoAlerta.Localizacao;
        request.Valor = null;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(LeituraSensorRequest.Latitude)));
    }

    [Fact]
    public void Validar_LatitudeForaDaFaixa_RetornaErroEmLatitude()
    {
        // Arrange
        var request = CriarValido();
        request.Latitude = 120;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(LeituraSensorRequest.Latitude)));
    }
}