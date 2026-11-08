using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig configuracao) : base(configuracao)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        var sql = """
            SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();
        while (await leitor.ReadAsync())
        {
            plataformas.Add(MapearPlataforma(leitor));
        }

        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        var sql = """
            SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? MapearPlataforma(leitor) : null;
    }

    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null)
    {
        var sql = """
            SELECT 1
            FROM tb_plataforma
            WHERE no_plataforma = @nome
              AND (@id_ignorar IS NULL OR co_seq_plataforma <> @id_ignorar)
            LIMIT 1
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@id_ignorar", idIgnorar);

        return await comando.ExecuteScalarAsync() is not null;
    }

    public async Task<bool> PossuiJogosAsync(int id)
    {
        var sql = """
            SELECT 1
            FROM tb_jogo
            WHERE co_plataforma = @id
            LIMIT 1
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        return await comando.ExecuteScalarAsync() is not null;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        var sql = """
            INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
            VALUES (@nome, @fabricante, @ano_lancamento)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano_lancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        var sql = """
            UPDATE tb_plataforma
            SET no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @ano_lancamento
            WHERE co_seq_plataforma = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", plataforma.Id);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano_lancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        var sql = """
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    private static Plataforma MapearPlataforma(MySqlDataReader leitor)
    {
        return new Plataforma
        {
            Id = leitor.GetInt32("co_seq_plataforma"),
            Nome = leitor.GetString("no_plataforma"),
            Fabricante = leitor.GetString("no_fabricante"),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
        };
    }
}
