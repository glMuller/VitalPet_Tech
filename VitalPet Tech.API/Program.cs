using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;
using Serilog;                                              // >>> NOVO
using HealthChecks.UI.Client;                                // >>> NOVO
using Microsoft.AspNetCore.Diagnostics.HealthChecks;         // >>> NOVO
using OpenTelemetry.Metrics;                                 // >>> NOVO
using OpenTelemetry.Trace;                                   // >>> NOVO

var builder = WebApplication.CreateBuilder(args);

// >>> NOVO: Logging estruturado com Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // substitui o logger padrão do ASP.NET pelo Serilog
// <<< FIM NOVO

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = 
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() {
        Title = "VitalPet Tech API",
        Version = "v1",
        Description = "API de saúde contínua do pet — FIAP Challenge 2026"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// >>> NOVO: Health Checks
builder.Services.AddHealthChecks()
    .AddOracle(
        connectionString: builder.Configuration.GetConnectionString("OracleConnection")!,
        name: "oracle-database",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "oracle" });
// <<< FIM NOVO

// >>> NOVO: OpenTelemetry (tracing + métricas)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("VitalPet_Tech.API")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());
   
// <<< FIM NOVO

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseSerilogRequestLogging(); // >>> NOVO: loga cada requisição HTTP (método, path, status, tempo)

app.MapControllers();

// >>> NOVO: endpoint /health
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
// <<< FIM NOVO

app.Run();

public partial class Program { } // >>> NOVO: necessário para o WebApplicationFactory nos testes de integração