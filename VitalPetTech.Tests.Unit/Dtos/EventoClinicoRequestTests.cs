using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Tests.Unit.Helpers;
using Xunit;

namespace VitalPetTech.Tests.Unit.Dtos;

public class EventoClinicoRequestTests
{
    private static EventoClinicoRequest CriarValido() => new()
    {
        Tipo = TipoEvento.Consulta,
        DataEvento = DateTime.Now,
        Clinica = "Clínica VitalPet",
        Descricao = "Consulta de rotina",
        IdPet = 1
    };

    [Fact]
    public void Validar_EventoValido_NaoRetornaErros()
    {
        // Arrange
        var request = CriarValido();

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Empty(erros);
    }

    [Fact]
    public void Validar_ClinicaVazia_RetornaErroEmClinica()
    {
        // Arrange
        var request = CriarValido();
        request.Clinica = "";

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(EventoClinicoRequest.Clinica)));
    }

    [Fact]
    public void Validar_TipoForaDoEnum_RetornaErroEmTipo()
    {
        // Arrange
        var request = CriarValido();
        request.Tipo = (TipoEvento)99;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(EventoClinicoRequest.Tipo)));
    }

    [Fact]
    public void Validar_ProximoRetornoNulo_NaoRetornaErros()
    {
        // Arrange — o retorno é opcional
        var request = CriarValido();
        request.ProximoRetorno = null;

        // Act
        var erros = ValidationHelper.Validar(request);

        // Assert
        Assert.Empty(erros);
    }
}