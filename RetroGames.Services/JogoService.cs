using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        int pagina = 1,
        int tamanhoPagina = 5,
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null)
    {
        IReadOnlyCollection<int>? idsJogos = null;
        if (idGenero.HasValue)
        {
            var vinculos = await repositorio.JogoGenero.ListarTodosAsync();
            idsJogos = vinculos.Where(vinculo => vinculo.IdGenero == idGenero.Value)
                .Select(vinculo => vinculo.IdJogo).Distinct().ToList();
        }

        // O Repository mantém a contagem e a paginação, inclusive para uma lista de ids vazia.
        var (jogos, total) = await repositorio.Jogo.ListarAsync(
            pagina, tamanhoPagina, busca, idPlataforma, idsJogos);
        return (await MontarDetalhesAsync(jogos), total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await ObterJogoAsync(id);
        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });
        return detalhes.Single();
    }

    public async Task<Jogo> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var generos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generos);
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return jogo;
    }

    public async Task AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var existente = await ObterJogoAsync(jogo.Id);
        var generos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generos);
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = existente.Ativo;
        jogo.DataCadastro = existente.DataCadastro;

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await repositorio.Jogo.AlterarAsync(jogo);
            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        await ObterJogoAsync(id);
        await repositorio.Jogo.DesativarAsync(id);
    }

    private async Task<Jogo> ObterJogoAsync(int id)
    {
        return await repositorio.Jogo.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
    }

    private async Task ValidarReferenciasAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (await repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaInexistente);
        }

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(condicao => condicao.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CondicaoInexistente);
        }

        var generos = await repositorio.Genero.ListarAsync();
        if (idsGeneros.Count == 0 || idsGeneros.Except(generos.Select(genero => genero.Id)).Any())
        {
            throw new BusinessRuleException(BusinessErrors.GeneroInexistente);
        }
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        // Quatro chamadas em lote; nenhuma consulta dentro da composição por jogo.
        var plataformas = await repositorio.Plataforma.ListarAsync();
        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        var generos = await repositorio.Genero.ListarAsync();
        var vinculos = await repositorio.JogoGenero.ListarPorJogosAsync(
            jogos.Select(jogo => jogo.Id).ToList());

        return (from jogo in jogos
                join plataforma in plataformas on jogo.IdPlataforma equals plataforma.Id
                join condicao in condicoes on jogo.IdCondicaoJogo equals condicao.Id
                select new JogoDetalheServiceModel
                {
                    Jogo = jogo,
                    NomePlataforma = plataforma.Nome,
                    DescricaoCondicao = condicao.Descricao,
                    Generos = (from vinculo in vinculos
                               join genero in generos on vinculo.IdGenero equals genero.Id
                               where vinculo.IdJogo == jogo.Id
                               select genero).ToList(),
                }).ToList();
    }
}
