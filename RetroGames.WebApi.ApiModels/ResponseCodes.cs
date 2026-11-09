namespace RetroGames.WebApi.ApiModels;

public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO = 1;

    public static readonly (int Code, string DefaultMessage) MODEL_NULL =
        (2, "O modelo veio nulo.");

    public static readonly (int Code, string DefaultMessage) MODEL_STATE_INVALIDO =
        (3, "Há um erro com o modelo enviado.");

    public static readonly (int Code, string DefaultMessage) NAO_AUTENTICADO =
        (4, "Faça login para continuar.");

    public static readonly (int Code, string DefaultMessage) ACESSO_NEGADO =
        (5, "Você não tem permissão para esta operação.");

    public static readonly (int Code, string DefaultMessage) CREDENCIAIS_INVALIDAS =
        (6, "E-mail ou senha inválidos.");

    public static readonly (int Code, string DefaultMessage) PLATAFORMA_NAO_ENCONTRADA =
        (7, "Plataforma não encontrada.");

    public static readonly (int Code, string DefaultMessage) PLATAFORMA_NOME_DUPLICADO =
        (8, "Já existe uma plataforma com este nome.");

    public static readonly (int Code, string DefaultMessage) PLATAFORMA_POSSUI_JOGOS =
        (9, "Não é possível excluir: existem jogos desta plataforma.");

    public static readonly (int Code, string DefaultMessage) JOGO_NAO_ENCONTRADO =
        (10, "Jogo não encontrado.");

    public static readonly (int Code, string DefaultMessage) PLATAFORMA_INEXISTENTE =
        (11, "A plataforma informada não existe.");

    public static readonly (int Code, string DefaultMessage) CONDICAO_INEXISTENTE =
        (12, "A condição informada não existe.");

    public static readonly (int Code, string DefaultMessage) GENERO_INEXISTENTE =
        (13, "Um ou mais gêneros informados não existem.");
}
