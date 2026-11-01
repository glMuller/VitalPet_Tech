using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Infrastructure.Data;
using VitalPetTech.Infrastructure.Repositories;
using Xunit;

namespace VitalPetTech.Tests.Unit.Repositories;

public class AlertaRepositoryTests
{
    private static async Task<(ApplicationContext Context, Pet Pet)> CriarCenarioAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationContext(options);

        var tutor = new Tutor { NomeTutor = "João", CpfTutor = "1", TelefoneTutor = "1", EnderecoTutor = "Rua" };
        context.Tutores.Add(tutor);
        await context.SaveChangesAsync();

        var pet = new Pet
        {
            Nome = "Rex", EspeciePet = "Cachorro", PesoPet = 10,
            DataNascimento = new DateTime(2022, 1, 1), IdTutor = tutor.Id
        };
        context.Pets.Add(pet);
        await context.SaveChangesAsync();

        context.Alertas.AddRange(
            new Alerta { IdPet = pet.Id, Resolvido = false, Timestamp = new DateTime(2026, 1, 1), TipoAlerta = TipoAlerta.Batimento, Severidade = Severidade.Info },
            new Alerta { IdPet = pet.Id, Resolvido = true, Timestamp = new DateTime(2026, 1, 2), TipoAlerta = TipoAlerta.Temperatura, Severidade = Severidade.Atencao },
            new Alerta { IdPet = pet.Id, Resolvido = false, Timestamp = new DateTime(2026, 1, 3), TipoAlerta = TipoAlerta.Localizacao, Severidade = Severidade.Critico });
        await context.SaveChangesAsync();

        return (context, pet);
    }

    [Fact]
    public async Task GetPagedAsync_FiltroResolvidoFalse_RetornaApenasPendentes()
    {
        // Arrange
        var (context, _) = await CriarCenarioAsync();
        var repositorio = new AlertaRepository(context);

        // Act
        var resultado = await repositorio.GetPagedAsync(new AlertaFilter { Resolvido = false });

        // Assert
        Assert.Equal(2, resultado.Items.Count);
        Assert.All(resultado.Items, a => Assert.False(a.Resolvido));
    }

    [Fact]
    public async Task GetPagedAsync_SemOrderBy_RetornaMaisRecentePrimeiro()
    {
        // Arrange
        var (context, _) = await CriarCenarioAsync();
        var repositorio = new AlertaRepository(context);

        // Act
        var resultado = await repositorio.GetPagedAsync(new AlertaFilter());

        // Assert
        Assert.Equal(new DateTime(2026, 1, 3), resultado.Items[0].Timestamp);
    }
}