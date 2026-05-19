using DotNet.Api.Data;
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

// Applies pending EF Core migrations when the application starts.
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