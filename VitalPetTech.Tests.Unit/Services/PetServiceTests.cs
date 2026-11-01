using Microsoft.Extensions.Logging;
using Moq;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Exceptions;
using VitalPetTech.Application.Services;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;
using Xunit;

namespace VitalPetTech.Tests.Unit.Services;

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _petRepository = new();
    private readonly Mock<ITutorRepository> _tutorRepository = new();

    private PetService CriarService(ILogger<PetService>? logger = null) =>
        new(_petRepository.Object, _tutorRepository.Object, logger ?? Mock.Of<ILogger<PetService>>());

    private static PetRequest CriarRequest(int idTutor = 1) => new()
    {
        Nome = "Rex",
        DataNascimento = new DateTime(2022, 1, 1),
        PesoPet = 10,
        EspeciePet = "Cachorro",
        IdTutor = idTutor
    };

    [Fact]
    public async Task CreateAsync_TutorExistente_AdicionaPetEDevolveNomeDoTutor()
    {
        // Arrange
        _tutorRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Tutor { Id = 1, NomeTutor = "João da Silva" });
        var service = CriarService();

        // Act
        var resposta = await service.CreateAsync(CriarRequest());

        // Assert
        Assert.Equal("João da Silva", resposta.NomeTutor);
        _petRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_TutorInexistente_LancaBusinessRuleExceptionENaoAdiciona()
    {
        // Arrange
        _tutorRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Tutor?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(CriarRequest(99)));
        _petRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_PetInexistente_LancaNotFoundException()
    {
        // Arrange
        _petRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Pet?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetByIdAsync_PetInexistente_RegistraLogDeWarning()
    {
        // Arrange
        _petRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Pet?)null);
        var loggerMock = new Mock<ILogger<PetService>>();
        var service = CriarService(loggerMock.Object);

        // Act
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));

        // Assert
        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_TrocandoParaTutorInexistente_LancaBusinessRuleExceptionENaoPersiste()
    {
        // Arrange
        var pet = new Pet { Id = 1, IdTutor = 1, Nome = "Rex", EspeciePet = "Cachorro" };
        _petRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pet);
        _tutorRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Tutor?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.UpdateAsync(1, CriarRequest(idTutor: 2)));
        _petRepository.Verify(r => r.UpdateAsync(It.IsAny<Pet>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_PetExistente_AplicaCamposEPersiste()
    {
        // Arrange
        var pet = new Pet { Id = 1, IdTutor = 1, Nome = "Antigo", EspeciePet = "Gato" };
        _petRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pet);
        var service = CriarService();

        // Act
        await service.UpdateAsync(1, CriarRequest());

        // Assert
        Assert.Equal("Rex", pet.Nome);
        Assert.Equal("Cachorro", pet.EspeciePet);
        _petRepository.Verify(r => r.UpdateAsync(pet), Times.Once);
    }
}