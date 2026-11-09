using System.Net;

namespace RetroGames.Services;

public class BusinessRuleException : Exception
{
    public (int Codigo, string Mensagem, HttpStatusCode StatusHttp) Erro { get; }

    public BusinessRuleException((int Codigo, string Mensagem, HttpStatusCode StatusHttp) erro)
        : base(erro.Mensagem)
    {
        Erro = erro;
    }
}
