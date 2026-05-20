using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VitalPet_Tech.API.Data;

var builder = WebApplication.CreateBuilder(args);
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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.MapControllers();


app.Run();