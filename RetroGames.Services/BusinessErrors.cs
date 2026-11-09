using System.Net;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.Services;

public static class BusinessErrors
{
    public static readonly (int Codigo, string Mensagem, HttpStatusCode StatusHttp) PlataformaNaoEncontrada =
        (ResponseCodes.PLATAFORMA_NAO_ENCONTRADA.Code,
         ResponseCodes.PLATAFORMA_NAO_ENCONTRADA.DefaultMessage,
         HttpStatusCode.NotFound);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode StatusHttp) PlataformaNomeDuplicado =
        (ResponseCodes.PLATAFORMA_NOME_DUPLICADO.Code,
         ResponseCodes.PLATAFORMA_NOME_DUPLICADO.DefaultMessage,
         HttpStatusCode.Conflict);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode StatusHttp) PlataformaPossuiJogos =
        (ResponseCodes.PLATAFORMA_POSSUI_JOGOS.Code,
         ResponseCodes.PLATAFORMA_POSSUI_JOGOS.DefaultMessage,
         HttpStatusCode.Conflict);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode StatusHttp) JogoNaoEncontrado =
        (ResponseCodes.JOGO_NAO_ENCONTRADO.Code,
         ResponseCodes.JOGO_NAO_ENCONTRADO.DefaultMessage,
         HttpStatusCode.NotFound);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode StatusHttp) PlataformaInexistente =
        (ResponseCodes.PLATAFORMA_INEXISTENTE.Code,
         ResponseCodes.PLATAFORMA_INEXISTENTE.DefaultMessage,
         HttpStatusCode.BadRequest);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode StatusHttp) CondicaoInexistente =
        (ResponseCodes.CONDICAO_INEXISTENTE.Code,
         ResponseCodes.CONDICAO_INEXISTENTE.DefaultMessage,
         HttpStatusCode.BadRequest);

    public static readonly (int Codigo, string Mensagem, HttpStatusCode StatusHttp) GeneroInexistente =
        (ResponseCodes.GENERO_INEXISTENTE.Code,
         ResponseCodes.GENERO_INEXISTENTE.DefaultMessage,
         HttpStatusCode.BadRequest);
}
