using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using VitalPet_Tech.Controllers;
using VitalPet_Tech.Entites;
using Xunit;

namespace VitalPetTech.Tests.Unit.Controllers;

public class PetControllerTests
{
    // Cada teste recebe um banco InMemory novo e isolado, identificado por um nome único
    private static ApplicationContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationContext(options);
    }

    // Método auxiliar para criar um Tutor válido, evitando repetição nos testes
    private static Tutor CriarTutorValido(int id = 1)
    {
        return new Tutor
        {
            Id = id,
            NomeTutor = "João da Silva",
            CpfTutor = "123.456.789-00",
            TelefoneTutor = "(11) 99999-0000",
            EnderecoTutor = "Rua Exemplo, 123"
        };
    }

    [Fact]
    public async Task GetById_PetExistente_RetornaOkComPet()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var tutor = CriarTutorValido();
        context.Tutor.Add(tutor);

        var pet = new Pet { Nome = "Rex", EspeciePet = "Cachorro", PesoPet = 10, DataNascimento = DateTime.Now, IdTutor = tutor.Id };
        context.Pets.Add(pet);
        await context.SaveChangesAsync();

        var controller = new PetController(context);

        // Act
        var resultado = await controller.GetById(pet.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var petRetornado = Assert.IsType<Pet>(okResult.Value);
        Assert.Equal("Rex", petRetornado.Nome);
    }

    [Fact]
    public async Task GetById_PetInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new PetController(context);

        // Act
        var resultado = await controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task Create_PetValido_RetornaCreatedAtActionEPersisteNoBanco()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var tutor = CriarTutorValido();
        context.Tutor.Add(tutor);
        await context.SaveChangesAsync();

        var controller = new PetController(context);
        var novoPet = new Pet { Nome = "Mimi", EspeciePet = "Gato", PesoPet = 4.5, DataNascimento = DateTime.Now, IdTutor = tutor.Id };

        // Act
        var resultado = await controller.Create(novoPet);

        // Assert
        Assert.IsType<CreatedAtActionResult>(resultado.Result);
        Assert.Equal(1, await context.Pets.CountAsync());
    }

    [Fact]
    public async Task Delete_PetExistente_RemoveDoBancoERetornaNoContent()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var tutor = CriarTutorValido();
        context.Tutor.Add(tutor);

        var pet = new Pet { Nome = "Bidu", EspeciePet = "Cachorro", PesoPet = 8, DataNascimento = DateTime.Now, IdTutor = tutor.Id };
        context.Pets.Add(pet);
        await context.SaveChangesAsync();
        var controller = new PetController(context);

        // Act
        var resultado = await controller.Delete(pet.Id);

        // Assert
        Assert.IsType<NoContentResult>(resultado);
        Assert.Equal(0, await context.Pets.CountAsync());
    }

    [Fact]
    public async Task Delete_PetInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var controller = new PetController(context);

        // Act
        var resultado = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task GetByEspecie_EspecieInexistente_RetornaNotFound()
    {
        // Arrange
        var context = CriarContextoEmMemoria();
        var tutor = CriarTutorValido();
        context.Tutor.Add(tutor);
        context.Pets.Add(new Pet { Nome = "Rex", EspeciePet = "Cachorro", PesoPet = 10, DataNascimento = DateTime.Now, IdTutor = tutor.Id });
        await context.SaveChangesAsync();
        var controller = new PetController(context);

        // Act
        var resultado = await controller.GetByEspecie("Hamster");

        // Assert
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }
}