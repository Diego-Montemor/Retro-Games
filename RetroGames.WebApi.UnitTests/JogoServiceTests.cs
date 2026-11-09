using System.Transactions;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> jogos = new();
    private readonly Mock<IJogoGeneroRepository> vinculos = new();
    private readonly Mock<IPlataformaRepository> plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> condicoes = new();
    private readonly Mock<IGeneroRepository> generos = new();
    private readonly JogoService servico;

    public JogoServiceTests()
    {
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(r => r.Jogo).Returns(jogos.Object);
        repositorio.SetupGet(r => r.JogoGenero).Returns(vinculos.Object);
        repositorio.SetupGet(r => r.Plataforma).Returns(plataformas.Object);
        repositorio.SetupGet(r => r.CondicaoJogo).Returns(condicoes.Object);
        repositorio.SetupGet(r => r.Genero).Returns(generos.Object);
        plataformas.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1 });
        plataformas.Setup(r => r.ListarAsync()).ReturnsAsync(new List<Plataforma>
        {
            new() { Id = 1, Nome = "Super Nintendo" },
        });
        condicoes.Setup(r => r.ListarAsync()).ReturnsAsync(new List<CondicaoJogo>
        {
            new() { Id = 2, Descricao = "Bom" },
        });
        generos.Setup(r => r.ListarAsync()).ReturnsAsync(new List<Genero>
        {
            new() { Id = 3, Nome = "Aventura" },
            new() { Id = 4, Nome = "Ação" },
        });
        vinculos.Setup(r => r.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>()))
            .ReturnsAsync(new List<JogoGenero>());
        servico = new JogoService(repositorio.Object);
    }

    private static Jogo CriarJogo(int id = 7) => new()
    {
        Id = id, Nome = "  Mario  ", IdPlataforma = 1, IdCondicaoJogo = 2,
        Preco = 100m, Estoque = 1, AnoLancamento = 1990,
    };

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public async Task ListarAsync_CompoeDetalhesEmLoteEPreservaTotal(int quantidade)
    {
        var pagina = Enumerable.Range(1, quantidade).Select(CriarJogo).ToList();
        jogos.Setup(r => r.ListarAsync(2, 20, "Mario", 1, null, false)).ReturnsAsync((pagina, 50));
        vinculos.Setup(r => r.ListarPorJogosAsync(It.Is<IReadOnlyCollection<int>>(ids => ids.Count == quantidade)))
            .ReturnsAsync(pagina.Select(jogo => new JogoGenero { IdJogo = jogo.Id, IdGenero = 3 }).ToList());

        var resultado = await servico.ListarAsync(2, 20, "Mario", 1);

        Assert.Equal(50, resultado.Total);
        Assert.Equal(quantidade, resultado.Jogos.Count);
        Assert.All(resultado.Jogos, detalhe =>
        {
            Assert.Equal("Super Nintendo", detalhe.NomePlataforma);
            Assert.Equal("Bom", detalhe.DescricaoCondicao);
            Assert.Equal("Aventura", Assert.Single(detalhe.Generos).Nome);
        });
        plataformas.Verify(r => r.ListarAsync(), Times.Once);
        condicoes.Verify(r => r.ListarAsync(), Times.Once);
        generos.Verify(r => r.ListarAsync(), Times.Once);
        vinculos.Verify(r => r.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>()), Times.Once);
        plataformas.Verify(r => r.BuscarPorIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(99, 0)]
    public async Task ListarAsync_GeneroEntregaIdsFiltradosAoRepository(int idGenero, int quantidadeIds)
    {
        vinculos.Setup(r => r.ListarTodosAsync()).ReturnsAsync(new List<JogoGenero>
        {
            new() { IdJogo = 7, IdGenero = 3 },
            new() { IdJogo = 8, IdGenero = 4 },
        });
        jogos.Setup(r => r.ListarAsync(2, 5, null, null, It.IsAny<IReadOnlyCollection<int>>(), false))
            .ReturnsAsync((new List<Jogo>(), 0));

        await servico.ListarAsync(pagina: 2, idGenero: idGenero);

        jogos.Verify(r => r.ListarAsync(2, 5, null, null,
            It.Is<IReadOnlyCollection<int>>(ids => ids.Count == quantidadeIds && (quantidadeIds == 0 || ids.Contains(7))),
            false), Times.Once);
    }

    [Fact]
    public async Task BuscarPorIdAsync_DevolveDetalheComTodosOsGeneros()
    {
        jogos.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(CriarJogo());
        vinculos.Setup(r => r.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>()))
            .ReturnsAsync(new List<JogoGenero>
            {
                new() { IdJogo = 7, IdGenero = 3 }, new() { IdJogo = 7, IdGenero = 4 },
            });

        var detalhe = await servico.BuscarPorIdAsync(7);

        Assert.Equal(7, detalhe.Jogo.Id);
        Assert.Equal(new[] { 3, 4 }, detalhe.Generos.Select(genero => genero.Id));
    }

    [Theory]
    [InlineData("buscar")]
    [InlineData("alterar")]
    [InlineData("desativar")]
    public async Task OperacaoAsync_JogoAusenteLancaErroDoCatalogo(string operacao)
    {
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => operacao switch
        {
            "buscar" => servico.BuscarPorIdAsync(7),
            "alterar" => servico.AlterarAsync(CriarJogo(), new[] { 3 }),
            _ => servico.DesativarAsync(7),
        });

        Assert.Equal(BusinessErrors.JogoNaoEncontrado, erro.Erro);
        jogos.Verify(r => r.AlterarAsync(It.IsAny<Jogo>()), Times.Never);
        jogos.Verify(r => r.DesativarAsync(It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData(false, "plataforma")]
    [InlineData(true, "plataforma")]
    [InlineData(false, "condicao")]
    [InlineData(true, "condicao")]
    [InlineData(false, "genero")]
    [InlineData(true, "genero")]
    [InlineData(false, "sem-genero")]
    [InlineData(true, "sem-genero")]
    public async Task GravarAsync_ReferenciaInvalidaImpedeTodaGravacao(bool alterar, string referencia)
    {
        var jogo = CriarJogo();
        jogos.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(CriarJogo());
        if (referencia == "plataforma") jogo.IdPlataforma = 99;
        if (referencia == "condicao") jogo.IdCondicaoJogo = 99;
        var ids = referencia == "sem-genero" ? Array.Empty<int>() : new[] { referencia == "genero" ? 99 : 3 };
        var esperado = referencia switch
        {
            "plataforma" => BusinessErrors.PlataformaInexistente,
            "condicao" => BusinessErrors.CondicaoInexistente,
            _ => BusinessErrors.GeneroInexistente,
        };

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            alterar ? servico.AlterarAsync(jogo, ids) : servico.InserirAsync(jogo, ids));

        Assert.Equal(esperado, erro.Erro);
        jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>()), Times.Never);
        jogos.Verify(r => r.AlterarAsync(It.IsAny<Jogo>()), Times.Never);
        vinculos.Verify(r => r.RemoverPorJogoAsync(It.IsAny<int>()), Times.Never);
        vinculos.Verify(r => r.InserirAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task GravarAsync_TransacaoConfirmaSomenteAposUltimaGravacao(bool alterar, bool falharVinculos)
    {
        var jogo = CriarJogo();
        var dataOriginal = new DateTime(2020, 1, 1);
        var existente = CriarJogo();
        existente.DataCadastro = dataOriginal;
        jogos.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(existente);
        var observador = new ObservadorTransacao();
        Transaction? transacao = null;
        var etapas = new List<string>();
        void ObservarGravacao()
        {
            transacao = Transaction.Current;
            Assert.NotNull(transacao);
            Assert.Equal(System.Transactions.IsolationLevel.ReadCommitted, transacao.IsolationLevel);
            transacao.EnlistVolatile(observador, EnlistmentOptions.None);
            etapas.Add("jogo");
        }
        jogos.Setup(r => r.InserirAsync(jogo)).Callback(ObservarGravacao).ReturnsAsync(42);
        jogos.Setup(r => r.AlterarAsync(jogo)).Callback(ObservarGravacao).Returns(Task.CompletedTask);
        vinculos.Setup(r => r.RemoverPorJogoAsync(7)).Callback(() =>
        {
            Assert.Same(transacao, Transaction.Current);
            etapas.Add("remover");
        }).Returns(Task.CompletedTask);
        vinculos.Setup(r => r.InserirAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>()))
            .Returns(async (int id, IReadOnlyCollection<int> ids) =>
            {
                await Task.Yield(); // Confere a propagação da transação após uma espera assíncrona.
                Assert.Same(transacao, Transaction.Current);
                Assert.Equal(alterar ? 7 : 42, id);
                Assert.Equal(new[] { 3, 4 }, ids);
                Assert.Null(observador.Estado);
                etapas.Add("vinculos");
                if (falharVinculos) throw new InvalidOperationException("Falha simulada nos vínculos.");
            });

        Task GravarAsync() => alterar
            ? servico.AlterarAsync(jogo, new[] { 3, 3, 4 })
            : servico.InserirAsync(jogo, new[] { 3, 3, 4 });
        if (falharVinculos)
            await Assert.ThrowsAsync<InvalidOperationException>(GravarAsync);
        else
            await GravarAsync();

        Assert.Equal(falharVinculos ? TransactionStatus.Aborted : TransactionStatus.Committed, observador.Estado);
        Assert.Equal(alterar ? new[] { "jogo", "remover", "vinculos" } : new[] { "jogo", "vinculos" }, etapas);
        Assert.Equal("Mario", jogo.Nome);
        if (alterar)
        {
            Assert.False(jogo.Ativo);
            Assert.Equal(dataOriginal, jogo.DataCadastro);
        }
        else
        {
            Assert.True(jogo.Ativo);
            Assert.Equal(DateTimeKind.Utc, jogo.DataCadastro.Kind);
        }
    }

    [Fact]
    public async Task DesativarAsync_PreservaVinculosEChamaSomenteDesativacao()
    {
        jogos.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(CriarJogo());

        await servico.DesativarAsync(7);

        jogos.Verify(r => r.DesativarAsync(7), Times.Once);
        vinculos.VerifyNoOtherCalls();
    }

    private sealed class ObservadorTransacao : IEnlistmentNotification
    {
        public TransactionStatus? Estado { get; private set; }
        public void Prepare(PreparingEnlistment preparacao) => preparacao.Prepared();
        public void Commit(Enlistment registro) { Estado = TransactionStatus.Committed; registro.Done(); }
        public void Rollback(Enlistment registro) { Estado = TransactionStatus.Aborted; registro.Done(); }
        public void InDoubt(Enlistment registro) { Estado = TransactionStatus.InDoubt; registro.Done(); }
    }
}
