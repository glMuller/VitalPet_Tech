using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Controllers;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Controllers;

public class TutorControllerTests
{
    private static ApplicationContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationContext(options);
    }

    private static Tutor CriarTutorValido()
    {
        return new Tutor
        {
            NomeTutor = "João da Silva",
            CpfTutor = "123.456.789-00",
            TelefoneTutor = "(11) 99999-0000",
            EnderecoTutor = "Rua Exemplo, 123"
        };
    }

    [Fact]
    public async Task GetById_TutorExistente_RetornaOkComTutor()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var tutor = CriarTutorValido();
        context.Tutor.Add(tutor);
        await context.SaveChangesAsync();

        var controller = new TutorController(context);

        // Act
        var resultado = await controller.GetById(tutor.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var tutorRetornado = Assert.IsType<Tutor>(okResult.Value);
        Assert.Equal("João da Silva", tutorRetornado.NomeTutor);
    }

    [Fact]
    public async Task GetById_TutorInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new TutorController(context);

        // Act
        var resultado = await controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task Create_TutorValido_RetornaCreatedAtActionEPersisteNoBanco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new TutorController(context);
        var novoTutor = CriarTutorValido();

        // Act
        var resultado = await controller.Create(novoTutor);

        // Assert
        Assert.IsType<CreatedAtActionResult>(resultado.Result);
        Assert.Equal(1, await context.Tutor.CountAsync());
    }

    [Fact]
    public async Task Delete_TutorExistente_RemoveDoBancoERetornaNoContent()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var tutor = CriarTutorValido();
        context.Tutor.Add(tutor);
        await context.SaveChangesAsync();
        var controller = new TutorController(context);

        // Act
        var resultado = await controller.Delete(tutor.Id);

        // Assert
        Assert.IsType<NoContentResult>(resultado);
        Assert.Equal(0, await context.Tutor.CountAsync());
    }

    [Fact]
    public async Task Delete_TutorInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new TutorController(context);

        // Act
        var resultado = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado);
    }
}