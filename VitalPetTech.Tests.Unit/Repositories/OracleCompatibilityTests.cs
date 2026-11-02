using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VitalPetTech.Infrastructure;
using VitalPetTech.Infrastructure.Auth;
using VitalPetTech.Infrastructure.Data;
using VitalPetTech.Infrastructure.Mongo;

namespace VitalPetTech.Tests.Unit.Repositories;

public class OracleCompatibilityTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConsultaDeExistencia_NaoGeraLiteraisBooleanosDoOracle23(bool designTime)
    {
        var services = new ServiceCollection();
        services.AddInfrastructure("Data Source=localhost:1521/FAKE;User Id=teste;Password=teste;",
            new JwtSettings(), new MongoSettings());
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        using var context = designTime
            ? new ApplicationContextFactory().CreateDbContext(Array.Empty<string>())
            : scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        // Gera SQL com EXISTS sem abrir uma conexão com o banco.
        var sql = context.Usuarios.Select(u => context.Usuarios.Any(other => other.Id == u.Id)).ToQueryString();

        Assert.Contains("EXISTS", sql, StringComparison.OrdinalIgnoreCase);
        Assert.False(Regex.IsMatch(sql, @"\b(TRUE|FALSE)\b", RegexOptions.IgnoreCase), sql);
    }
}
