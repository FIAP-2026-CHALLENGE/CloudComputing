using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotNet.Api.Data;

/// <summary>
/// Fábrica usada SOMENTE pelas ferramentas de design-time do EF Core
/// (dotnet ef migrations add/script). Nunca é chamada em runtime —
/// o Program.cs tem sua própria configuração real, lida do ambiente/appsettings.
/// Existe porque 'dotnet ef' instancia o AppDbContext isoladamente, sem rodar
/// o Program.cs inteiro, então não tem acesso à connection string real do .env.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Connection string "fake" — nunca conecta de verdade durante 'migrations add',
        // só precisa ter um formato válido pra o Pomelo montar o SQL.
        const string designTimeConnectionString =
            "Server=localhost;Port=3306;Database=petcare;User=root;Password=designtime;";

        optionsBuilder.UseMySql(designTimeConnectionString, new MySqlServerVersion(new Version(8, 0, 0)));

        return new AppDbContext(optionsBuilder.Options);
    }
}