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

public class TutorServiceTests
{
    private readonly Mock<ITutorRepository> _repository = new();

    private TutorService CriarService() =>
        new(_repository.Object, Mock.Of<ILogger<TutorService>>());

    private static TutorRequest CriarRequest(string nome = "João da Silva") => new()
    {
        NomeTutor = nome,
        CpfTutor = "123.456.789-00",
        TelefoneTutor = "(11) 99999-0000",
        EnderecoTutor = "Rua Exemplo, 123"
    };

    [Fact]
    public async Task GetByIdAsync_TutorExistente_RetornaResponseComResumoDosPets()
    {
        // Arrange
        var tutor = new Tutor
        {
            Id = 1,
            NomeTutor = "João da Silva",
            Pets = new List<Pet> { new() { Id = 5, Nome = "Rex", EspeciePet = "Cachorro" } }
        };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tutor);
        var service = CriarService();

        // Act
        var resposta = await service.GetByIdAsync(1);

        // Assert
        Assert.Equal("João da Silva", resposta.NomeTutor);
        Assert.Single(resposta.Pets);
    }

    [Fact]
    public async Task GetByIdAsync_TutorInexistente_LancaNotFoundException()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Tutor?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task CreateAsync_RequestValido_AdicionaTutorNoRepositorio()
    {
        // Arrange
        var service = CriarService();

        // Act
        var resposta = await service.CreateAsync(CriarRequest());

        // Assert
        Assert.Equal("João da Silva", resposta.NomeTutor);
        _repository.Verify(r => r.AddAsync(It.Is<Tutor>(t => t.NomeTutor == "João da Silva")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_TutorExistente_AplicaCamposEPersiste()
    {
        // Arrange
        var tutor = new Tutor { Id = 1, NomeTutor = "Antigo" };
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tutor);
        var service = CriarService();

        // Act
        await service.UpdateAsync(1, CriarRequest("Novo Nome"));

        // Assert
        Assert.Equal("Novo Nome", tutor.NomeTutor);
        _repository.Verify(r => r.UpdateAsync(tutor), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_TutorInexistente_LancaNotFoundENaoRemove()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Tutor?)null);
        var service = CriarService();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(999));
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Tutor>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_PageSizeAcimaDoLimite_NormalizaAntesDeConsultar()
    {
        // Arrange
        _repository.Setup(r => r.GetPagedAsync(It.IsAny<TutorFilter>()))
            .ReturnsAsync(new PagedResult<Tutor>(new List<Tutor>(), 1, 50, 0));
        var service = CriarService();
        var filtro = new TutorFilter { PageSize = 500 };

        // Act
        await service.GetPagedAsync(filtro);

        // Assert
        _repository.Verify(r => r.GetPagedAsync(It.Is<TutorFilter>(f => f.PageSize == 50)), Times.Once);
    }
}