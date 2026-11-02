using Microsoft.Extensions.Logging;
using Moq;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Exceptions;
using VitalPetTech.Application.Services;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using Xunit;

namespace VitalPetTech.Tests.Unit.Services;

public class AlertaServiceTests
{
    private readonly Mock<IAlertaRepository> _alertaRepository = new();
    private readonly Mock<IPetRepository> _petRepository = new();

    private AlertaService CriarService() =>
        new(_alertaRepository.Object, _petRepository.Object, Mock.Of<ILogger<AlertaService>>());

    [Fact]
    public async Task CreateAsync_PetExistente_AlertaNasceNaoResolvido()
    {
        // Arrange
        _petRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Pet { Id = 1, Nome = "Rex" });
        Alerta? salvo = null;
        _alertaRepository.Setup(r => r.AddAsync(It.IsAny<Alerta>()))
            .Callback<Alerta>(a => salvo = a)
            .Returns(Task.CompletedTask);
        var service = CriarService();
        var request = new AlertaRequest
        {
            TipoAlerta = TipoAlerta.Temperatura,
            Valor = 39.8,
            Severidade = Severidade.Critico,
            Timestamp = DateTime.Now,
            IdPet = 1
        };

        // Act
        var resposta = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(salvo);
        Assert.False(salvo!.Resolvido);
        Assert.False(resposta.Resolvido);
    }

    [Fact]
    public async Task CreateAsync_PetInexistente_LancaBusinessRuleException()
    {
        // Arrange
        _petRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pet?)null);
        var service = CriarService();
        var request = new AlertaRequest { Timestamp = DateTime.Now, IdPet = 99 };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(request));
        _alertaRepository.Verify(r => r.AddAsync(It.IsAny<Alerta>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_AlertaExistente_MarcaComoResolvido()
    {
        // Arrange
        var alerta = new Alerta { Id = 1, IdPet = 1, Resolvido = false, Timestamp = DateTime.Now };
        _alertaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(alerta);
        var service = CriarService();
        var request = new AlertaUpdateRequest
        {
            TipoAlerta = TipoAlerta.Batimento,
            Severidade = Severidade.Atencao,
            Timestamp = DateTime.Now,
            IdPet = 1,
            Resolvido = true
        };

        // Act
        await service.UpdateAsync(1, request);

        // Assert
        Assert.True(alerta.Resolvido);
        _alertaRepository.Verify(r => r.UpdateAsync(alerta), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_FiltroResolvidoFalse_RepassaFiltroAoRepositorio()
    {
        // Arrange
        _alertaRepository.Setup(r => r.GetPagedAsync(It.IsAny<AlertaFilter>()))
            .ReturnsAsync(new PagedResult<Alerta>(new List<Alerta>(), 1, 10, 0));
        var service = CriarService();

        // Act
        await service.GetPagedAsync(new AlertaFilter { Resolvido = false });

        // Assert
        _alertaRepository.Verify(r => r.GetPagedAsync(It.Is<AlertaFilter>(f => f.Resolvido == false)), Times.Once);
    }
}