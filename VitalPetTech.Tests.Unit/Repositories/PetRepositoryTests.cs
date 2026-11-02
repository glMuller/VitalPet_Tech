using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Infrastructure.Data;
using VitalPetTech.Infrastructure.Repositories;
using Xunit;

namespace VitalPetTech.Tests.Unit.Repositories;

public class PetRepositoryTests
{
    private static ApplicationContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationContext(options);
    }

    private static async Task<Tutor> CriarTutorAsync(ApplicationContext context)
    {
        var tutor = new Tutor { NomeTutor = "João", CpfTutor = "1", TelefoneTutor = "1", EnderecoTutor = "Rua" };
        context.Tutores.Add(tutor);
        await context.SaveChangesAsync();
        return tutor;
    }

    private static Pet NovoPet(int tutorId, string nome, string especie, double peso) => new()
    {
        Nome = nome,
        EspeciePet = especie,
        PesoPet = peso,
        DataNascimento = new DateTime(2022, 1, 1),
        IdTutor = tutorId
    };

    [Fact]
    public async Task GetPagedAsync_FiltroPorEspecie_RetornaApenasDaEspecieIgnorandoMaiusculas()
    {
        // Arrange
        var context = CriarContexto();
        var tutor = await CriarTutorAsync(context);
        context.Pets.AddRange(
            NovoPet(tutor.Id, "Mimi", "Gato", 4),
            NovoPet(tutor.Id, "Rex", "Cachorro", 10),
            NovoPet(tutor.Id, "Tom", "Gato", 5));
        await context.SaveChangesAsync();
        var repositorio = new PetRepository(context);

        // Act
        var resultado = await repositorio.GetPagedAsync(new PetFilter { Especie = "gato" });

        // Assert
        Assert.Equal(2, resultado.Items.Count);
        Assert.All(resultado.Items, p => Assert.Equal("Gato", p.EspeciePet));
    }

    [Fact]
    public async Task GetPagedAsync_UltimaPagina_RetornaItensRestantesETotais()
    {
        // Arrange
        var context = CriarContexto();
        var tutor = await CriarTutorAsync(context);
        for (var i = 1; i <= 12; i++)
            context.Pets.Add(NovoPet(tutor.Id, $"Pet{i:00}", "Cachorro", 10));
        await context.SaveChangesAsync();
        var repositorio = new PetRepository(context);

        // Act
        var resultado = await repositorio.GetPagedAsync(new PetFilter { Page = 3, PageSize = 5 });

        // Assert
        Assert.Equal(2, resultado.Items.Count);
        Assert.Equal(12, resultado.TotalCount);
        Assert.Equal(3, resultado.TotalPages);
    }

    [Fact]
    public async Task GetPagedAsync_OrdenacaoPorPesoDescendente_RetornaMaisPesadoPrimeiro()
    {
        // Arrange
        var context = CriarContexto();
        var tutor = await CriarTutorAsync(context);
        context.Pets.AddRange(
            NovoPet(tutor.Id, "A", "Cachorro", 5),
            NovoPet(tutor.Id, "B", "Cachorro", 20),
            NovoPet(tutor.Id, "C", "Cachorro", 10));
        await context.SaveChangesAsync();
        var repositorio = new PetRepository(context);

        // Act
        var resultado = await repositorio.GetPagedAsync(new PetFilter { OrderBy = "peso", Descending = true });

        // Assert
        Assert.Equal(20, resultado.Items[0].PesoPet);
    }

    [Fact]
    public async Task GetPagedAsync_OrderByInvalido_UsaOrdenacaoPadraoPorNome()
    {
        // Arrange
        var context = CriarContexto();
        var tutor = await CriarTutorAsync(context);
        context.Pets.AddRange(
            NovoPet(tutor.Id, "Zeca", "Cachorro", 10),
            NovoPet(tutor.Id, "Ana", "Gato", 4));
        await context.SaveChangesAsync();
        var repositorio = new PetRepository(context);

        // Act
        var resultado = await repositorio.GetPagedAsync(new PetFilter { OrderBy = "nome; DROP TABLE Pets" });

        // Assert
        Assert.Equal("Ana", resultado.Items[0].Nome);
    }
}