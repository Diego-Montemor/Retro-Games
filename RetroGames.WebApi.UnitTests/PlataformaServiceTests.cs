using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> plataformaRepositorio = new();
    private readonly PlataformaService servico;

    public PlataformaServiceTests()
    {
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(r => r.Plataforma).Returns(plataformaRepositorio.Object);
        servico = new PlataformaService(repositorio.Object);
    }

    [Fact]
    public async Task InserirAsync_NormalizaNomeEDevolveIdentificadorGerado()
    {
        var plataforma = new Plataforma { Nome = "  Super Nintendo  " };
        plataformaRepositorio.Setup(r => r.InserirAsync(It.Is<Plataforma>(p => p.Nome == "Super Nintendo")))
            .ReturnsAsync(7);

        var resultado = await servico.InserirAsync(plataforma);

        Assert.Equal(7, resultado.Id);
        Assert.Equal("Super Nintendo", resultado.Nome);
        plataformaRepositorio.Verify(r => r.ExisteComNomeAsync("Super Nintendo", null), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GravarAsync_NomeDuplicadoImpedeGravacao(bool alterar)
    {
        var plataforma = new Plataforma { Id = 7, Nome = "  Nintendo  " };
        plataformaRepositorio.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(plataforma);
        plataformaRepositorio.Setup(r => r.ExisteComNomeAsync("Nintendo", alterar ? 7 : null))
            .ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            alterar ? servico.AlterarAsync(plataforma) : servico.InserirAsync(plataforma));

        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, erro.Erro);
        plataformaRepositorio.Verify(r => r.InserirAsync(It.IsAny<Plataforma>()), Times.Never);
        plataformaRepositorio.Verify(r => r.AlterarAsync(It.IsAny<Plataforma>()), Times.Never);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GravarAsync_DuplicidadeConcorrenteViraErroDeNegocio(bool alterar)
    {
        var plataforma = new Plataforma { Id = 7, Nome = "Nintendo" };
        var duplicidade = new RegistroDuplicadoException(new Exception("Duplicidade simulada."));
        plataformaRepositorio.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(plataforma);
        plataformaRepositorio.Setup(r => r.InserirAsync(plataforma)).ThrowsAsync(duplicidade);
        plataformaRepositorio.Setup(r => r.AlterarAsync(plataforma)).ThrowsAsync(duplicidade);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            alterar ? servico.AlterarAsync(plataforma) : servico.InserirAsync(plataforma));

        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, erro.Erro);
    }

    [Fact]
    public async Task AlterarAsync_ManterProprioNomeNaoImpedeAlteracao()
    {
        var plataforma = new Plataforma { Id = 7, Nome = "  Nintendo  " };
        plataformaRepositorio.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(plataforma);
        // Simula o contrato do Repository: o próprio id não conta como duplicidade.
        plataformaRepositorio.Setup(r => r.ExisteComNomeAsync("Nintendo", null)).ReturnsAsync(true);
        plataformaRepositorio.Setup(r => r.ExisteComNomeAsync("Nintendo", 7)).ReturnsAsync(false);

        await servico.AlterarAsync(plataforma);

        plataformaRepositorio.Verify(r => r.AlterarAsync(
            It.Is<Plataforma>(p => p.Id == 7 && p.Nome == "Nintendo")), Times.Once);
    }

    [Theory]
    [InlineData("buscar")]
    [InlineData("alterar")]
    [InlineData("excluir")]
    public async Task OperacaoAsync_PlataformaAusenteLancaErroSemGravar(string operacao)
    {
        var plataforma = new Plataforma { Id = 7, Nome = "Nintendo" };

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => operacao switch
        {
            "buscar" => servico.BuscarPorIdAsync(7),
            "alterar" => servico.AlterarAsync(plataforma),
            _ => servico.ExcluirAsync(7),
        });

        Assert.Equal(BusinessErrors.PlataformaNaoEncontrada, erro.Erro);
        plataformaRepositorio.Verify(r => r.AlterarAsync(It.IsAny<Plataforma>()), Times.Never);
        plataformaRepositorio.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_PlataformaComJogosImpedeExclusao()
    {
        plataformaRepositorio.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(new Plataforma { Id = 7 });
        plataformaRepositorio.Setup(r => r.PossuiJogosAsync(7)).ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => servico.ExcluirAsync(7));

        Assert.Equal(BusinessErrors.PlataformaPossuiJogos, erro.Erro);
        plataformaRepositorio.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_PlataformaSemJogosPermiteExclusao()
    {
        plataformaRepositorio.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(new Plataforma { Id = 7 });

        await servico.ExcluirAsync(7);

        plataformaRepositorio.Verify(r => r.ExcluirAsync(7), Times.Once);
    }
}
