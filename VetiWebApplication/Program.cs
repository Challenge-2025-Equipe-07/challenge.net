using VetiWebApplication.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

// IRealizando a injeção da dependencia de DBCONTEXT
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString, b => b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
     }); 
builder.Services.AddEndpointsApiExplorer();

// Gerando o arquivo OpenAPI que o Scalar vai ler
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancelationToken) =>
    {
        document.Info.Title = "Veti - Sistema de Acompanhamento Clínico de Pets";
        document.Info.Version = "v1";
        document.Info.Description = " Esta é a documentação daAPI para gerenciamento de consultas, exames e tratamentos veterinários.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Ativando o endpoint do OpenAPI
    app.MapOpenApi();
    app.MapScalarApiReference();

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();