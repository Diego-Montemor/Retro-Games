namespace RetroGames.Repository;

public class RepositorySet : IRepositorySet
{
    public RepositorySet(
        IPlataformaRepository plataforma,
        IJogoRepository jogo,
        IJogoGeneroRepository jogoGenero,
        IGeneroRepository genero,
        ICondicaoJogoRepository condicaoJogo)
    {
        Plataforma = plataforma;
        Jogo = jogo;
        JogoGenero = jogoGenero;
        Genero = genero;
        CondicaoJogo = condicaoJogo;
    }

    public IPlataformaRepository Plataforma { get; }
    public IJogoRepository Jogo { get; }
    public IJogoGeneroRepository JogoGenero { get; }
    public IGeneroRepository Genero { get; }
    public ICondicaoJogoRepository CondicaoJogo { get; }
}
