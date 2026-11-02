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

public class LeituraSensorServiceTests
{
    private readonly Mock<ILeituraSensorRepository> _repository = new();
    private readonly Mock<IPetRepository> _petRepository = new();

    private LeituraSensorService CriarService() =>
        new(_repository.Object, _petRepository.Object, Mock.Of<ILogger<LeituraSensorService>>());

    private static LeituraSensorRequest CriarRequest(int idPet = 1) => new()
    {
        IdPet = idPet,
        Tipo = TipoAlerta.Temperatura,
        Valor = 38.5,
        Timestamp = DateTime.Now
    };

    [Fact]
    public async Task CreateAsync_PetExistente_GravaLeituraNoRepositorio()
    {
        // Arrange
        _petRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        LeituraSensor? salva = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<LeituraSensor>()))
            .Callback<LeituraSensor>(l => salva = l)
            .Returns(Task.CompletedTask);
        var service = CriarService();

        // Act
        var resposta = await service.CreateAsync(CriarRequest());

        // Assert
        Assert.NotNull(salva);
        Assert.Equal(38.5, salva!.Valor);
        Assert.Equal(1, resposta.IdPet);
    }

    [Fact]
    public async Task CreateAsync_PetInexistente_LancaBusinessRuleExceptionENaoGrava()
    {
        // Arrange
        _petRepository.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(CriarRequest(99)));
        _repository.Verify(r => r.AddAsync(It.IsAny<LeituraSensor>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_LeituraInexistente_LancaNotFoundException()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync((LeituraSensor?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync("abc"));
    }

    [Fact]
    public async Task GetPagedAsync_PageSizeAcimaDoLimite_NormalizaAntesDeConsultar()
    {
        // Arrange
        _repository.Setup(r => r.GetPagedAsync(It.IsAny<LeituraSensorFilter>()))
            .ReturnsAsync(new PagedResult<LeituraSensor>(new List<LeituraSensor>(), 1, 50, 0));
        var service = CriarService();

        // Act
        await service.GetPagedAsync(new LeituraSensorFilter { PageSize = 500 });

        // Assert
        _repository.Verify(r => r.GetPagedAsync(It.Is<LeituraSensorFilter>(f => f.PageSize == 50)), Times.Once);
    }
}