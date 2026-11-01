using Microsoft.Extensions.Logging;
using Moq;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Exceptions;
using VitalPetTech.Application.Services;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using Xunit;

namespace VitalPetTech.Tests.Unit.Services;

public class EventoClinicoServiceTests
{
    private readonly Mock<IEventoClinicoRepository> _eventoRepository = new();
    private readonly Mock<IPetRepository> _petRepository = new();

    private EventoClinicoService CriarService() =>
        new(_eventoRepository.Object, _petRepository.Object, Mock.Of<ILogger<EventoClinicoService>>());

    private static EventoClinicoRequest CriarRequest(int idPet = 1) => new()
    {
        Tipo = TipoEvento.Vacina,
        DataEvento = DateTime.Now,
        Clinica = "Clínica VitalPet",
        Descricao = "Vacina antirrábica",
        IdPet = idPet
    };

    [Fact]
    public async Task CreateAsync_PetExistente_AdicionaEventoEDevolveNomeDoPet()
    {
        // Arrange
        _petRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Pet { Id = 1, Nome = "Rex" });
        var service = CriarService();

        // Act
        var resposta = await service.CreateAsync(CriarRequest());

        // Assert
        Assert.Equal("Rex", resposta.NomePet);
        _eventoRepository.Verify(r => r.AddAsync(It.IsAny<EventoClinico>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PetInexistente_LancaBusinessRuleExceptionENaoAdiciona()
    {
        // Arrange
        _petRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pet?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(CriarRequest(99)));
        _eventoRepository.Verify(r => r.AddAsync(It.IsAny<EventoClinico>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_EventoInexistente_LancaNotFoundException()
    {
        // Arrange
        _eventoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((EventoClinico?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }
}