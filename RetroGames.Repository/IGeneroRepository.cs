using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarAsync();
}
