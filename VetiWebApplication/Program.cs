using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Repositories;
using VetiWebApplication.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

// Realizando a injeção da dependencia de DBCONTEXT
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


// Registra os repositórios e serviços para serem utilizados através da injeção de dependência.
builder.Services.AddScoped<ITutorRepository, TutorRepository>();
builder.Services.AddScoped<TutorService>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();
builder.Services.AddScoped<VeterinarioService>();
builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<IExameRepository, ExameRepository>();
builder.Services.AddScoped<ExameService>();
builder.Services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
builder.Services.AddScoped<MedicamentoService>();
builder.Services.AddScoped<IExameMedicamentoRepository, ExameMedicamentoRepository>();
builder.Services.AddScoped<ExameMedicamentoService>();
builder.Services.AddScoped<ITratamentoRepository, TratamentoRepository>();
builder.Services.AddScoped<ITratamentoMedicamentoRepository, TratamentoMedicamentoRepository>();
builder.Services.AddScoped<TratamentoService>();

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