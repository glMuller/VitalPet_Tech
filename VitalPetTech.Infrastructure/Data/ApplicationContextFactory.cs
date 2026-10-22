using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VitalPetTech.Infrastructure.Data;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("VITALPET_ORACLE_CONNECTION")
            ?? "Data Source=localhost:1521/XEPDB1;User Id=design;Password=design;";

        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseOracle(connectionString)
            .Options;

        return new ApplicationContext(options);
    }
}