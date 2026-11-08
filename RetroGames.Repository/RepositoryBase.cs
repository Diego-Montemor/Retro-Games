using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _configuracao;

    protected RepositoryBase(RepositoryConfig configuracao)
    {
        _configuracao = configuracao;
    }

    // O Repository que chamar este método deve receber a conexão com await using.
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_configuracao.StringConexao);

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            // Se a abertura falhar, a conexão ainda não foi entregue ao chamador.
            await conexao.DisposeAsync();
            throw;
        }
    }

    // O chamador também deve receber o comando com await using.
    protected MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException excecao) when (excecao.Number == 1062)
        {
            throw new RegistroDuplicadoException(excecao);
        }
    }
}
