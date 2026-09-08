using DotNet.Api.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotNet.Api.Infraestrutura.Health;

/// <summary>
/// Health check customizado que valida a conectividade real com o banco MySQL,
/// usado pelo endpoint nativo /health.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
  private readonly AppDbContext _context;

  public DatabaseHealthCheck(AppDbContext context)
  {
    _context = context;
  }

  public async Task<HealthCheckResult> CheckHealthAsync(
      HealthCheckContext context,
      CancellationToken cancellationToken = default)
  {
    try
    {
      var podeConectar = await _context.Database.CanConnectAsync(cancellationToken);

      return podeConectar
          ? HealthCheckResult.Healthy("Conexão com o MySQL Database estabelecida com sucesso.")
          : HealthCheckResult.Unhealthy("Não foi possível conectar ao MySQL Database.");
    }
    catch (Exception ex)
    {
      return HealthCheckResult.Unhealthy("Falha ao verificar a conexão com o MySQL Database.", ex);
    }
  }
}