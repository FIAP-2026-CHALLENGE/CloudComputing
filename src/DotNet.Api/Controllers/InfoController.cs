using DotNet.Api.Aplicacao;
using DotNet.Api.Hateoas;
using DotNet.Infrastructure.Mongo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

/// <summary>
/// Raiz da API: nome, versão e ambiente, com links para os recursos principais.
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("")]
[Produces("application/json")]
public class InfoController : ControllerBase
{
    private readonly IWebHostEnvironment _ambiente;
    private readonly IConfiguration _configuracao;

    public InfoController(IWebHostEnvironment ambiente, IConfiguration configuracao)
    {
        _ambiente = ambiente;
        _configuracao = configuracao;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Recurso<InformacoesApiResponse>), 200)]
    public ActionResult<Recurso<InformacoesApiResponse>> Get()
    {
        var informacoes = new InformacoesApiResponse(InformacoesApi.Nome, InformacoesApi.Versao, _ambiente.EnvironmentName);

        var links = new List<LinkHateoas>
        {
            new("login", Url.Action("Login", "Auth")!, "POST"),
            new("responsaveis", Url.Action("GetAll", "Responsaveis")!, "GET"),
            new("animais", Url.Action("GetAll", "Animais")!, "GET"),
            new("care-events", Url.Action("GetAll", "CareEvents")!, "GET"),
            new("health", "/health", "GET")
        };

        // Relatos e /health/mongo só aparecem quando o MongoDB está configurado.
        if (MongoConfiguracao.EstaHabilitado(_configuracao))
        {
            links.Add(new("relatos", Url.Action("GetAll", "Relatos")!, "GET"));
            links.Add(new("health-mongo", "/health/mongo", "GET"));
        }

        return Ok(new Recurso<InformacoesApiResponse>(informacoes, links));
    }
}
