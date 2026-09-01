using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Controllers;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Controllers;

public class AlertaControllerTests
{
    private static ApplicationContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationContext(options);
    }

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
    public async Task GetById_AlertaExistente_RetornaOkComAlerta()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);

        var alerta = new Alerta
        {
            TipoAlerta = TipoAlerta.Batimento,
            Valor = 130,
            Severidade = Severidade.Critico,
            Timestamp = DateTime.Now,
            IdPet = pet.Id
        };
        context.Alerta.Add(alerta);
        await context.SaveChangesAsync();

        var controller = new AlertaController(context);

        // Act
        var resultado = await controller.GetById(alerta.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var alertaRetornado = Assert.IsType<Alerta>(okResult.Value);
        Assert.Equal(TipoAlerta.Batimento, alertaRetornado.TipoAlerta);
    }

    [Fact]
    public async Task GetById_AlertaInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new AlertaController(context);

        // Act
        var resultado = await controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task Create_AlertaValido_NasceComoNaoResolvidoEPersisteNoBanco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);
        var controller = new AlertaController(context);

        var novoAlerta = new Alerta
        {
            TipoAlerta = TipoAlerta.Temperatura,
            Valor = 39.8,
            Severidade = Severidade.Atencao,
            Timestamp = DateTime.Now,
            IdPet = pet.Id,
            Resolvido = true // o controller deve forçar isso para false ao criar
        };

        // Act
        var resultado = await controller.Create(novoAlerta);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
        var alertaCriado = Assert.IsType<Alerta>(createdResult.Value);
        Assert.False(alertaCriado.Resolvido);
    }

    [Fact]
    public async Task GetPendentes_ApenasAlertasNaoResolvidos_RetornaOkComListaFiltrada()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);

        context.Alerta.Add(new Alerta { TipoAlerta = TipoAlerta.Batimento, Severidade = Severidade.Critico, Timestamp = DateTime.Now, IdPet = pet.Id, Resolvido = false });
        context.Alerta.Add(new Alerta { TipoAlerta = TipoAlerta.Temperatura, Severidade = Severidade.Info, Timestamp = DateTime.Now, IdPet = pet.Id, Resolvido = true });
        await context.SaveChangesAsync();

        var controller = new AlertaController(context);

        // Act
        var resultado = await controller.GetPendentes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var alertas = Assert.IsAssignableFrom<IEnumerable<Alerta>>(okResult.Value);
        Assert.Single(alertas);
    }

    [Fact]
    public async Task Delete_AlertaExistente_RemoveDoBancoERetornaNoContent()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var pet = await CriarPetComTutorAsync(context);

        var alerta = new Alerta
        {
            TipoAlerta = TipoAlerta.Localizacao,
            Latitude = -23.5,
            Longitude = -46.6,
            Severidade = Severidade.Info,
            Timestamp = DateTime.Now,
            IdPet = pet.Id
        };
        context.Alerta.Add(alerta);
        await context.SaveChangesAsync();
        var controller = new AlertaController(context);

        // Act
        var resultado = await controller.Delete(alerta.Id);

        // Assert
        Assert.IsType<NoContentResult>(resultado);
        Assert.Equal(0, await context.Alerta.CountAsync());
    }

    [Fact]
    public async Task Delete_AlertaInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new AlertaController(context);

        // Act
        var resultado = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado);
    }
}