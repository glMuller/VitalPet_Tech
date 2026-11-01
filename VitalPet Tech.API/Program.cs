using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using VitalPet_Tech.API.Handlers;
using VitalPetTech.Application;
using VitalPetTech.Infrastructure;
using VitalPetTech.Infrastructure.Auth;
using VitalPet_Tech.API.Health;
using VitalPetTech.Infrastructure.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Logging estruturado
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Tratamento global de exceções
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// JWT: a chave vem de user-secrets (dev) ou variável de ambiente Jwt__Key (produção)
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
    throw new InvalidOperationException(
        "Configure 'Jwt:Key' com no mínimo 32 caracteres (dotnet user-secrets ou variável de ambiente Jwt__Key).");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "VitalPet Tech API",
        Version = "v1",
        Description = "API de saúde contínua do pet — FIAP Challenge 2026"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Botão "Authorize" no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe apenas o token JWT (obtido em POST /api/Auth/login)."
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// Camadas da Clean Architecture
var connectionString = builder.Configuration.GetConnectionString("OracleConnection")
    ?? throw new InvalidOperationException("A connection string 'OracleConnection' não foi configurada.");

builder.Services.AddApplication();
var mongo = builder.Configuration.GetSection("Mongo").Get<MongoSettings>() ?? new MongoSettings();
if (string.IsNullOrWhiteSpace(mongo.ConnectionString))
    throw new InvalidOperationException(
        "Configure 'Mongo:ConnectionString' (dotnet user-secrets ou variável de ambiente Mongo__ConnectionString).");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString, jwt, mongo);

// Health Checks
builder.Services.AddHealthChecks()
    .AddOracle(
        connectionString: connectionString,
        name: "oracle-database",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "oracle" })
    .AddCheck<MongoHealthCheck>(
        name: "mongodb",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "mongodb" });

// OpenTelemetry (tracing + métricas)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("VitalPet_Tech.API")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// O health check fica público, para orquestradores e monitoramento conseguirem consultar
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program { } // necessário para o WebApplicationFactory nos testes de integração