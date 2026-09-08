using DotNet.Api.Aplicacao.Middlewares;
using DotNet.Api.Data;
using DotNet.Api.Infraestrutura.Health;
using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Api.Models;
using DotNet.Api.Repositorios;
using DotNet.Api.Servicos;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
  var builder = WebApplication.CreateBuilder(args);

  builder.Host.UseSerilog((context, services, configuration) => configuration
      .MinimumLevel.Information()
      .Enrich.FromLogContext()
      .WriteTo.Console(
          outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}")
      .WriteTo.File(
          path: "logs/app-.log",
          rollingInterval: RollingInterval.Day,
          outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}"));

  builder.Services.AddControllers();

  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();

  // MySQL + EF Core.
  builder.Services.AddDbContext<AppDbContext>(options =>
  {
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new InvalidOperationException("Connection string 'MySqlConnection' was not found.");
    }

    // Versão fixa (não AutoDetect) para não depender de um banco vivo durante
    // operações de design-time do EF Core (dotnet ef migrations add/script).
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)), mySqlOptions =>
      {
        mySqlOptions.EnableRetryOnFailure(
        maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null);
      });
  });

  builder.Services.AddScoped<IResponsavelRepositorio, ResponsavelRepositorio>();
  builder.Services.AddScoped<IAnimalRepositorio, AnimalRepositorio>();
  builder.Services.AddScoped<ICareEventRepositorio, CareEventRepositorio>();

  builder.Services.AddScoped<IResponsavelServico, ResponsavelServico>();
  builder.Services.AddScoped<IAnimalServico, AnimalServico>();
  builder.Services.AddScoped<ICareEventServico, CareEventServico>();

  builder.Services
      .AddHealthChecks()
      .AddCheck<DatabaseHealthCheck>("mysql_database");

  builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(AplicacaoMetricas.NomeServico))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddSource(AplicacaoMetricas.NomeServico)
          .AddConsoleExporter())
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddMeter(AplicacaoMetricas.NomeServico)
          .AddConsoleExporter());

  var app = builder.Build();

  // Aplica as migrations pendentes e insere os dados de seed na primeira execução.
  // Necessário porque o container MySQL nasce vazio a cada 'docker compose up' na Sprint Cloud
  // (diferente do Oracle da FIAP, que já existe populado e por isso não precisa disso).
  using (var scope = app.Services.CreateScope())
  {
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var migrationLogger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("DatabaseMigration");

    try
    {
      migrationLogger.LogInformation("Applying database migrations...");
      dbContext.Database.Migrate();
      migrationLogger.LogInformation("Database migrations applied successfully.");

      if (dbContext.Responsaveis.Count() == 0)
      {
        migrationLogger.LogInformation("Inserting seed data...");

        var r1 = Responsavel.Criar("Ana Paula Souza", "ana.souza@email.com", "(11) 91234-5678", "123.456.789-00");
        var r2 = Responsavel.Criar("Carlos Eduardo Lima", "carlos.lima@email.com", "(21) 99876-5432", "987.654.321-00");
        dbContext.Responsaveis.AddRange(r1, r2);
        dbContext.SaveChanges();

        var a1 = Animal.Criar(r1.Id, "Bolinha", "Boli", "DOG", "Labrador",
            new DateTime(2020, 5, 20, 0, 0, 0, DateTimeKind.Utc), 28.5m, "MALE", "RGA-001");
        var a2 = Animal.Criar(r2.Id, "Mia", "Miau", "CAT", "Siamese",
            new DateTime(2021, 8, 3, 0, 0, 0, DateTimeKind.Utc), 4.2m, "FEMALE", "RGA-002");
        dbContext.Animais.AddRange(a1, a2);
        dbContext.SaveChanges();

        dbContext.CareEvents.AddRange(
            CareEvent.Criar(a1.Id, "VACCINE", "Vacina V10 anual",
                "Aplicacao anual da vacina polivalente V10.",
                new DateTime(2025, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                new DateTime(2025, 6, 1, 9, 30, 0, DateTimeKind.Utc),
                "COMPLETED", "HIGH", "Bolinha reagiu bem."),
            CareEvent.Criar(a2.Id, "CHECKUP", "Check-up semestral Mia",
                "Consulta de rotina com exames de sangue e urina.",
                new DateTime(2025, 7, 15, 14, 0, 0, DateTimeKind.Utc),
                null, "PENDING", "MEDIUM", "Agendar jejum de 8h antes.")
        );
        dbContext.SaveChanges();

        migrationLogger.LogInformation("Seed data inserted successfully.");
      }
    }
    catch (Exception ex)
    {
      migrationLogger.LogError(ex, "Error while applying database migrations.");
      throw;
    }
  }

  // Swagger fica sempre ativo (mesmo em Production) porque o container roda com
  // ASPNETCORE_ENVIRONMENT=Production e o Swagger é a ferramenta usada para
  // demonstrar as evidências de CRUD no vídeo de entrega.
  app.UseSwagger();
  app.UseSwaggerUI();

  app.UseMiddleware<CorrelationIdMiddleware>();
  app.UseSerilogRequestLogging();

  app.UseAuthorization();

  app.MapControllers();

  app.MapHealthChecks("/health");

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "A aplicação DotNet.Api falhou ao iniciar.");
}
finally
{
  Log.CloseAndFlush();
}

// Exposto como classe pública parcial para permitir o uso de WebApplicationFactory<Program>
// em testes de integração, caso sejam adicionados futuramente.
public partial class Program
{
}