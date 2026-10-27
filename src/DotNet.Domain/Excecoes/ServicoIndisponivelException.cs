namespace DotNet.Domain.Excecoes;

/// <summary>
/// Lançada quando um recurso depende de um serviço externo desligado por configuração
/// (ex.: relatos do tutor sem MongoDB). O GlobalExceptionMiddleware converte esta exceção em 503.
/// </summary>
public class ServicoIndisponivelException : Exception
{
    public ServicoIndisponivelException(string mensagem) : base(mensagem)
    {
    }
}
