using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig configuracao) : base(configuracao)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        var sql = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await LerVinculosAsync(leitor);
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos)
    {
        if (idsJogos.Count == 0)
        {
            return new List<JogoGenero>();
        }

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);
        var nomesParametros = new List<string>();
        var indice = 0;
        foreach (var idJogo in idsJogos)
        {
            var nomeParametro = "@id_jogo_" + indice++;
            nomesParametros.Add(nomeParametro);
            AdicionarParametro(comando, nomeParametro, idJogo);
        }

        comando.CommandText = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN (
            """ + string.Join(", ", nomesParametros) + ") ORDER BY co_jogo, co_genero";
        await using var leitor = await comando.ExecuteReaderAsync();

        return await LerVinculosAsync(leitor);
    }

    public async Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);
        AdicionarParametro(comando, "@id_jogo", idJogo);
        var valores = new List<string>();
        var indice = 0;
        foreach (var idGenero in idsGeneros)
        {
            var nomeParametro = "@id_genero_" + indice++;
            valores.Add("(@id_jogo, " + nomeParametro + ")");
            AdicionarParametro(comando, nomeParametro, idGenero);
        }

        comando.CommandText = "INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES "
            + string.Join(", ", valores);
        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        var sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @id_jogo
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id_jogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }

    private static async Task<List<JogoGenero>> LerVinculosAsync(MySqlDataReader leitor)
    {
        var vinculos = new List<JogoGenero>();
        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero"),
            });
        }

        return vinculos;
    }
}
