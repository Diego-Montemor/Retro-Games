using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        int pagina = 1,
        int tamanhoPagina = 5,
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);
    Task<Jogo> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);
    Task AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);
    Task DesativarAsync(int id);
}
