using DotNet.Domain.Consultas;
using DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;
using DotNet.Domain.Repositorios;

namespace DotNet.Infrastructure.Repositorios;

/// <summary>
/// Repositório usado quando o MongoDB está desligado por configuração: toda operação
/// lança ServicoIndisponivelException, e a API responde 503 nas rotas de relatos.
/// </summary>
public class RelatoTutorRepositorioIndisponivel : IRelatoTutorRepositorio
{
    private const string Mensagem = "Relatos do tutor indisponíveis: o MongoDB não está configurado neste ambiente.";

    public Task<RelatoTutor?> ObterPorIdAsync(string id) => throw new ServicoIndisponivelException(Mensagem);

    public Task<ResultadoPaginado<RelatoTutor>> ListarPaginadoAsync(FiltroRelatoTutor filtro, ParametrosPaginacao paginacao)
        => throw new ServicoIndisponivelException(Mensagem);

    public Task AdicionarAsync(RelatoTutor relato) => throw new ServicoIndisponivelException(Mensagem);

    public Task AtualizarAsync(RelatoTutor relato) => throw new ServicoIndisponivelException(Mensagem);

    public Task RemoverAsync(RelatoTutor relato) => throw new ServicoIndisponivelException(Mensagem);
}
