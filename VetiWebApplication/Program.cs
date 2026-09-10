using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;
using VetiWebApplication.Data;
using VetiWebApplication.HealthChecks;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Repositories;
using VetiWebApplication.Services;


// Configuração do Serilog: define onde e como os logs serão gravados.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Nível mínimo: Information, Warning, Error
    .WriteTo.Console() // Mostra os logs no terminal
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Salva em arquivo, um por dia
    .Enrich.FromLogContext() // Permite enriquecer os logs com dados extras (usado para correlação de requisições)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry: Tracing (rastreia o caminho de cada requisição)
// e Métricas (tempo de resposta, contagem de requisições, taxa de erros).
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation(); // Rastreia cada requisição HTTP recebida pela API
        tracing.AddConsoleExporter(); // Exibe o rastreamento no terminal
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation(); // Métricas de requisições HTTP (duração, contagem, status code)
        metrics.AddConsoleExporter(); // Exibe as métricas no terminal
    });

// Substitui o sistema de logging padrão do ASP.NET Core pelo Serilog
builder.Host.UseSerilog();

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


// Health Checks: verifica se a API está de pé E se consegue conectar no Oracle.
builder.Services.AddHealthChecks()
    .AddCheck<OracleHealthCheck>("oracle-database", tags: new[] { "db", "oracle" });

var app = builder.Build();

// Loga automaticamente cada requisição HTTP: método, rota, status code e tempo de resposta.
// Isso também gera um identificador que permite correlacionar todos os logs de uma mesma requisição.
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    // Ativando o endpoint do OpenAPI
    app.MapOpenApi();
    app.MapScalarApiReference();

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

try
{
    Log.Information("Iniciando a aplicação Veti...");
    app.Run();
}
catch (Exception excecao)
{
    Log.Fatal(excecao, "A aplicação encerrou inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}