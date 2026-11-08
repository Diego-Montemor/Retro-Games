using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig configuracao) : base(configuracao)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        int pagina = 1,
        int tamanhoPagina = 5,
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyCollection<int>? ids = null,
        bool incluirInativos = false)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);
        var filtros = new List<string> { "(@incluir_inativos = TRUE OR st_ativo = TRUE)" };
        AdicionarParametro(comando, "@incluir_inativos", incluirInativos);

        if (!string.IsNullOrWhiteSpace(busca))
        {
            filtros.Add("no_jogo LIKE @busca");
            AdicionarParametro(comando, "@busca", "%" + busca + "%");
        }

        if (idPlataforma.HasValue)
        {
            filtros.Add("co_plataforma = @id_plataforma");
            AdicionarParametro(comando, "@id_plataforma", idPlataforma.Value);
        }

        if (ids is not null)
        {
            if (ids.Count == 0)
            {
                filtros.Add("1 = 0");
            }
            else
            {
                var nomesParametros = new List<string>();
                var indice = 0;
                foreach (var id in ids)
                {
                    // Só o nome do parâmetro é gerado; o ID nunca entra no texto SQL.
                    var nomeParametro = "@id_" + indice++;
                    nomesParametros.Add(nomeParametro);
                    AdicionarParametro(comando, nomeParametro, id);
                }

                filtros.Add("co_seq_jogo IN (" + string.Join(", ", nomesParametros) + ")");
            }
        }

        // A contagem e a página reutilizam exatamente o mesmo filtro e seus parâmetros.
        var filtroSql = string.Join(" AND ", filtros);
        comando.CommandText = "SELECT COUNT(*) FROM tb_jogo WHERE " + filtroSql;
        var total = Convert.ToInt32(await comando.ExecuteScalarAsync());

        comando.CommandText = """
            SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
                   qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
            FROM tb_jogo
            WHERE
            """ + " " + filtroSql + " ORDER BY no_jogo, co_seq_jogo LIMIT @tamanho_pagina OFFSET @deslocamento";
        AdicionarParametro(comando, "@tamanho_pagina", tamanhoPagina);
        AdicionarParametro(comando, "@deslocamento", checked((pagina - 1) * tamanhoPagina));

        await using var leitor = await comando.ExecuteReaderAsync();
        var jogos = new List<Jogo>();
        while (await leitor.ReadAsync())
        {
            jogos.Add(MapearJogo(leitor));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        var sql = """
            SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
                   qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? MapearJogo(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        var sql = """
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
                 st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES
                (@nome, @descricao, @ano_lancamento, @preco, @estoque,
                 @ativo, @data_cadastro, @id_plataforma, @id_condicao_jogo)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarDadosJogo(comando, jogo);
        AdicionarParametro(comando, "@data_cadastro", jogo.DataCadastro);

        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        var sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @ano_lancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                st_ativo = @ativo,
                co_plataforma = @id_plataforma,
                co_condicao_jogo = @id_condicao_jogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarDadosJogo(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        var sql = """
            UPDATE tb_jogo
            SET st_ativo = FALSE
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private void AdicionarDadosJogo(MySqlCommand comando, Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano_lancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@id_plataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@id_condicao_jogo", jogo.IdCondicaoJogo);
    }

    private static Jogo MapearJogo(MySqlDataReader leitor)
    {
        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo")) ? null : leitor.GetString("ds_jogo"),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo"),
        };
    }
}
