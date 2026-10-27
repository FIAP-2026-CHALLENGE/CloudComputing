using Microsoft.Extensions.Configuration;

namespace DotNet.Infrastructure.Mongo;

/// <summary>
/// O MongoDB é opcional: só fica ligado quando ConnectionStrings:MongoConnection está preenchida.
/// Sem ela (ex.: deploy de Cloud, que só aceita o Oracle), a API não registra o cliente,
/// o repositório real nem o health check do Mongo.
/// </summary>
public static class MongoConfiguracao
{
    public const string NomeConnectionString = "MongoConnection";

    public static bool EstaHabilitado(IConfiguration configuration)
    {
        return !string.IsNullOrWhiteSpace(configuration.GetConnectionString(NomeConnectionString));
    }
}
