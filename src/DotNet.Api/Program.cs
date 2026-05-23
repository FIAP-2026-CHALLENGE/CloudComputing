using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI documentation.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Oracle + EF Core.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'OracleConnection' was not found.");
    }

    options.UseOracle(connectionString);
});

var app = builder.Build();

// Applies pending EF Core migrations and seeds initial data when the application starts.
// This is required because the Oracle container starts empty on first execution.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("DatabaseMigration");

    try
    {
        logger.LogInformation("Applying database migrations...");
        dbContext.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");

        // Seed inicial — só insere se o banco estiver vazio
        if (!dbContext.Responsaveis.Any())
        {
            logger.LogInformation("Inserting seed data...");

            // 2 Responsaveis
            var r1 = new Responsavel
            {
                Name      = "Ana Paula Souza",
                Email     = "ana.souza@email.com",
                Phone     = "(11) 91234-5678",
                Cpf       = "123.456.789-00",
                CreatedAt = DateTime.UtcNow,
                IsActive  = true
            };
            var r2 = new Responsavel
            {
                Name      = "Carlos Eduardo Lima",
                Email     = "carlos.lima@email.com",
                Phone     = "(21) 99876-5432",
                Cpf       = "987.654.321-00",
                CreatedAt = DateTime.UtcNow,
                IsActive  = true
            };
            dbContext.Responsaveis.AddRange(r1, r2);
            dbContext.SaveChanges();

            // 2 Animais — usando os IDs gerados pelo Oracle
            var a1 = new Animal
            {
                ResponsavelId = r1.Id,
                Name          = "Bolinha",
                Nickname      = "Boli",
                Species       = "DOG",
                Breed         = "Labrador",
                BirthDate     = new DateTime(2020, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                Weight        = 28.5m,
                Sex           = "MALE",
                Rga           = "RGA-001",
                CreatedAt     = DateTime.UtcNow,
                IsActive      = true
            };
            var a2 = new Animal
            {
                ResponsavelId = r2.Id,
                Name          = "Mia",
                Nickname      = "Miau",
                Species       = "CAT",
                Breed         = "Siamese",
                BirthDate     = new DateTime(2021, 8, 3, 0, 0, 0, DateTimeKind.Utc),
                Weight        = 4.2m,
                Sex           = "FEMALE",
                Rga           = "RGA-002",
                CreatedAt     = DateTime.UtcNow,
                IsActive      = true
            };
            dbContext.Animais.AddRange(a1, a2);
            dbContext.SaveChanges();

            // 2 CareEvents — usando os IDs gerados pelo Oracle
            dbContext.CareEvents.AddRange(
                new CareEvent
                {
                    AnimalId      = a1.Id,
                    Type          = "VACCINE",
                    Title         = "Vacina V10 anual",
                    Description   = "Aplicacao anual da vacina polivalente V10.",
                    ScheduledDate = new DateTime(2025, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                    CompletedDate = new DateTime(2025, 6, 1, 9, 30, 0, DateTimeKind.Utc),
                    Status        = "COMPLETED",
                    Priority      = "HIGH",
                    Notes         = "Bolinha reagiu bem.",
                    CreatedAt     = DateTime.UtcNow,
                    IsActive      = true
                },
                new CareEvent
                {
                    AnimalId      = a2.Id,
                    Type          = "CHECKUP",
                    Title         = "Check-up semestral Mia",
                    Description   = "Consulta de rotina com exames de sangue e urina.",
                    ScheduledDate = new DateTime(2025, 7, 15, 14, 0, 0, DateTimeKind.Utc),
                    Status        = "PENDING",
                    Priority      = "MEDIUM",
                    Notes         = "Agendar jejum de 8h antes.",
                    CreatedAt     = DateTime.UtcNow,
                    IsActive      = true
                }
            );
            dbContext.SaveChanges();

            logger.LogInformation("Seed data inserted successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while applying database migrations.");
        throw;
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// Temporarily disabled to avoid HTTPS redirect issues during local development.
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();