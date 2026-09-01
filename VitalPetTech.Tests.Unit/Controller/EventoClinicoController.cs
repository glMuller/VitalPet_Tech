using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Controllers;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Controllers;

public class EventoClinicoControllerTests
{
    private static ApplicationContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationContext(options);
    }

    // Monta a cadeia Tutor -> Pet, necessária para criar um Evento Clínico válido
    private static async Task<Pet> CriarPetComTutorAsync(ApplicationContext context)
    {
        var tutor = new Tutor
        {
            NomeTutor = "João da Silva",
            CpfTutor = "123.456.789-00",
            TelefoneTutor = "(11) 99999-0000",
            EnderecoTutor = "Rua Exemplo, 123"
        };
        context.Tutor.Add(tutor);

        var pet = new Pet
        {
            Nome = "Rex",
            EspeciePet = "Cachorro",
            PesoPet = 10,
            DataNascimento = DateTime.Now,
            IdTutor = tutor.Id
        };
        context.Pets.Add(pet);

        await context.SaveChangesAsync();
        return pet;
    }

    [Fact]
    public async Task GetById_EventoExistente_RetornaOkComEvento()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);

        var evento = new EventoClinico
        {
            Tipo = TipoEvento.Consulta,
            DataEvento = DateTime.Now,
            Clinica = "Clínica VitalPet",
            Descricao = "Consulta de rotina",
            IdPet = pet.Id
        };
        context.EventoClinico.Add(evento);
        await context.SaveChangesAsync();

        var controller = new EventoClinicoController(context);

        // Act
        var resultado = await controller.GetById(evento.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var eventoRetornado = Assert.IsType<EventoClinico>(okResult.Value);
        Assert.Equal("Clínica VitalPet", eventoRetornado.Clinica);
    }

    [Fact]
    public async Task GetById_EventoInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new EventoClinicoController(context);

        // Act
        var resultado = await controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task Create_EventoValido_RetornaCreatedAtActionEPersisteNoBanco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);
        var controller = new EventoClinicoController(context);

        var novoEvento = new EventoClinico
        {
            Tipo = TipoEvento.Vacina,
            DataEvento = DateTime.Now,
            Clinica = "Clínica VitalPet",
            Descricao = "Vacina antirrábica",
            IdPet = pet.Id
        };

        // Act
        var resultado = await controller.Create(novoEvento);

        // Assert
        Assert.IsType<CreatedAtActionResult>(resultado.Result);
        Assert.Equal(1, await context.EventoClinico.CountAsync());
    }

    [Fact]
    public async Task GetByPet_PetSemEventos_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);
        var controller = new EventoClinicoController(context);

        // Act
        var resultado = await controller.GetByPet(pet.Id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task Delete_EventoExistente_RemoveDoBancoERetornaNoContent()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);

        var evento = new EventoClinico
        {
            Tipo = TipoEvento.Exame,
            DataEvento = DateTime.Now,
            Clinica = "Clínica VitalPet",
            Descricao = "Exame de sangue",
            IdPet = pet.Id
        };
        context.EventoClinico.Add(evento);
        await context.SaveChangesAsync();
        var controller = new EventoClinicoController(context);

        // Act
        var resultado = await controller.Delete(evento.Id);

        // Assert
        Assert.IsType<NoContentResult>(resultado);
        Assert.Equal(0, await context.EventoClinico.CountAsync());
    }

    [Fact]
    public async Task Delete_EventoInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new EventoClinicoController(context);

        // Act
        var resultado = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado);
    }
}