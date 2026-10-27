using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DotNet.Api.Aplicacao;
using DotNet.Api.Hateoas;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

/// <summary>
/// Sobe a API sem a connection string do MongoDB, como no deploy de Cloud (só Oracle).
/// </summary>
public class MongoDesligadoWebApplicationFactory : CustomWebApplicationFactory
{
    protected override bool MongoHabilitado => false;
}

[CollectionDefinition("Mongo Desligado")]
public class MongoDesligadoCollection : ICollectionFixture<MongoDesligadoWebApplicationFactory>
{
}

[Collection("Mongo Desligado")]
public class MongoDesligadoTests
{
    private readonly MongoDesligadoWebApplicationFactory _factory;

    public MongoDesligadoTests(MongoDesligadoWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_SemMongo_DeveContinuarRetornandoOk()
    {
        // Act
        var response = await _factory.CreateClient().GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthMongo_SemMongo_NaoDeveExistir()
    {
        // Act
        var response = await _factory.CreateClient().GetAsync("/health/mongo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Relatos_SemMongo_DeveRetornar503()
    {
        // Arrange
        var client = _factory.CriarClienteAutenticado();

        // Act
        var response = await client.GetAsync("/api/relatos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task Raiz_SemMongo_NaoDeveTerLinksDoMongo()
    {
        // Act
        var raiz = await _factory.CreateClient().GetFromJsonAsync<Recurso<InformacoesApiResponse>>("/");

        // Assert
        raiz!.Links.Should().NotContain(l => l.Rel == "relatos" || l.Rel == "health-mongo");
        raiz.Links.Should().Contain(l => l.Rel == "animais");
    }

    [Fact]
    public async Task Swagger_SemMongo_NaoDeveExibirRotasNemDescricaoDoMongo()
    {
        // Act
        var documento = await _factory.CreateClient().GetFromJsonAsync<JsonElement>("/swagger/v1/swagger.json");

        // Assert
        var rotas = documento.GetProperty("paths").EnumerateObject().Select(p => p.Name).ToList();
        rotas.Should().NotContain(r => r.StartsWith("/api/relatos"));
        rotas.Should().Contain("/api/animais");
        documento.GetProperty("info").GetProperty("description").GetString().Should().NotContainEquivalentOf("mongo");
    }

    [Fact]
    public async Task Animais_SemMongo_DevemFuncionarNormalmente()
    {
        // Arrange
        var client = _factory.CriarClienteAutenticado();

        // Act
        var response = await client.GetAsync("/api/animais");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
