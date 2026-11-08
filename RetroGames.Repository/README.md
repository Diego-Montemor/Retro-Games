# Base do Repository

`RepositoryConfig.StringConexao` guarda somente o texto de conexão. Conforme a etapa 2 da aula, o valor fica no `.env` local, ignorado pelo Git. Na aula 4, a API lerá esse arquivo e fornecerá a configuração; nesta etapa nada lê o `.env`. O arquivo versionado `.env.example` contém apenas o modelo sem senha real.

`RepositoryBase` oferece quatro métodos protegidos para os repositórios que herdarão dela:

- `AbrirConexaoAsync`: abre e devolve a conexão; descarta a conexão se a abertura falhar.
- `CriarComando`: cria um comando com o SQL recebido e a conexão aberta.
- `AdicionarParametro`: adiciona o nome e o valor ao comando; converte `null` em `DBNull.Value`.
- `ExecutarGravacaoAsync`: executa INSERT/UPDATE e devolve a quantidade de linhas afetadas. Somente o erro MySQL 1062 vira `RegistroDuplicadoException`, preservando a exceção original em `InnerException`.

Quem chama os métodos é responsável por descartar os recursos. Exemplo de uso para uma consulta nas próximas etapas (não é um Repository concreto da aplicação):

```csharp
await using var conexao = await AbrirConexaoAsync();
await using var comando = CriarComando(conexao, sql);
AdicionarParametro(comando, "@id", id);
await using var leitor = await comando.ExecuteReaderAsync();
// Ler e mapear as colunas para o modelo.
```

Para uma gravação, mantenha os mesmos `await using` da conexão e do comando e chame `ExecutarGravacaoAsync(comando)`. Em um INSERT, o chamador pode consultar `comando.LastInsertedId` antes do descarte. O leitor só é criado em consultas. Os `await using` descartam os recursos mesmo se houver erro.

A base não possui SQL de tabelas, regras de negócio nem transações. Os repositórios concretos terão SQL parametrizado de uma tabela por consulta; o Service tratará regras de negócio e transações.

Para conferir a compilação, execute na pasta `Retro-Games`:

```bash
dotnet build RetroGames.sln
```

## Verificação desta etapa

A solução compilou com zero erros e zero avisos. Uma aplicação temporária fora do repositório exercitou a base no MariaDB 11.8.8 autorizado pelo estudante, usando apenas uma tabela temporária. Foram confirmados: uma linha inserida, preservação de um parâmetro com aspas, conversão de `null` para SQL NULL, conversão de 1062 para `RegistroDuplicadoException` com `InnerException`, propagação do erro 1064 como `MySqlException` e fechamento da conexão. A tabela temporária desapareceu com o encerramento da sessão; os dados da aplicação não foram alterados. Essa validação não substitui a execução no MySQL 8 previsto na arquitetura.

Referência: [API básica do MySqlConnector](https://mysqlconnector.net/tutorials/basic-api/).

## Repository de plataformas — etapa 3

`IPlataformaRepository` define sete métodos assíncronos implementados por `PlataformaRepository`: `ListarAsync`, `BuscarPorIdAsync`, `ExisteComNomeAsync`, `PossuiJogosAsync`, `InserirAsync`, `AlterarAsync` e `ExcluirAsync`.

Cada comando acessa uma única tabela, sem JOIN ou subconsulta. `PossuiJogosAsync` lê somente `tb_jogo`; os outros leem ou gravam `tb_plataforma`. Os valores entram por `AdicionarParametro`, inclusive o ID opcional a ignorar. Os métodos de existência usam `SELECT 1 ... LIMIT 1`: uma linha indica verdadeiro, e nenhuma linha indica falso.

`MapearPlataforma` centraliza a correspondência entre as quatro colunas e as propriedades. INSERT e UPDATE passam por `ExecutarGravacaoAsync`; o INSERT retorna `LastInsertedId` convertido para `int`. DELETE executa o comando diretamente. Não há conferência de regras de negócio, exceção de negócio ou transação no Repository. Conexões, comandos e leitores usam `await using`.

Antes da implementação, os comandos do arquivo `database/007_validar_sql_plataforma_repository.sql` foram executados no MariaDB 11.8.8 autorizado, pelo cliente de terminal (sem Workbench). Todos produziram o resultado esperado e as gravações SQL foram desfeitas com ROLLBACK.

Depois da implementação, uma aplicação temporária fora do repositório exercitou os sete métodos reais. Foram confirmados: ordenação, mapeamento completo, retorno nulo para ID inexistente, nome com e sem ID a ignorar, consultas com e sem jogos, parâmetros contendo aspas, ID gerado, atualização dos três campos, exclusão, conversão de duplicidade em INSERT e UPDATE pela base e propagação do erro de FK 1451. A plataforma de teste foi removida em `finally` e a listagem final confirmou as seis plataformas originais. O build da solução terminou com zero erros e zero avisos. A execução no Workbench e no MySQL 8 não foi realizada.

## Repositories de jogos — etapa 4

`IJogoRepository` e `JogoRepository` oferecem `ListarAsync`, `BuscarPorIdAsync`, `InserirAsync`, `AlterarAsync` e `DesativarAsync`. `IJogoGeneroRepository` e `JogoGeneroRepository` oferecem `ListarTodosAsync`, `ListarPorJogosAsync`, `InserirAsync` e `RemoverPorJogoAsync`.

`ListarAsync` devolve uma tupla `(Jogos, Total)`. O padrão é página 1 com cinco itens e somente jogos ativos. A listagem usa nome e código como ordenação estável. `COUNT(*)` e a consulta com `LIMIT`/`OFFSET` são executados no SQL com exatamente o mesmo filtro e os mesmos parâmetros. `Total` é a quantidade encontrada antes da paginação, inclusive quando a página pedida está vazia.

Exemplo de chamada para as próximas etapas:

```csharp
var (jogos, total) = await repositorio.ListarAsync(
    pagina: 2, tamanhoPagina: 5, busca: "Mario", idPlataforma: 1);
```

A busca nula, vazia ou só com espaços não aplica filtro por nome; `%` e `_` têm o significado de curingas do LIKE. Plataforma nula não aplica filtro. `ids: null` não restringe os códigos, enquanto uma coleção vazia retorna zero jogos. Isso permite ao futuro Service enviar os IDs obtidos pelo filtro de gênero sem perder a contagem e a paginação no banco. Os nomes de parâmetros do IN são gerados por índice (`@id_0`, `@id_1` etc.); os valores sempre são adicionados por `AdicionarParametro`, sem concatenação no SQL. A validação de página, tamanho e permissões ficará nas camadas apropriadas da API.

O jogo é mapeado pelas dez colunas, incluindo `ds_jogo` anulável. A inserção usa a data recebida no modelo; a alteração preserva `dt_cadastro` e atualiza os demais dados. `DesativarAsync` somente atualiza `st_ativo`, mantendo registro e vínculos. Não existe DELETE de jogo.

Os vínculos de vários jogos são lidos em um único SELECT com IN. Coleção de jogos vazia retorna uma lista vazia. A inserção de gêneros usa um único INSERT com múltiplas linhas; coleção de gêneros vazia não gera comando. `RemoverPorJogoAsync` remove apenas os vínculos do ID informado, para posterior regravação. As duplicidades passam pelo tratamento da base. Cada consulta acessa uma tabela, sem JOIN ou subconsulta, e nenhum dos dois repositórios abre transações ou combina entidades. A transação de salvar jogo e gêneros caberá ao Service.

Antes da implementação, foi executado `database/008_validar_sql_jogos_repositories.sql` no MariaDB 11.8.8 autorizado, pelo cliente de terminal, sem Workbench. Foram conferidos: listagem sem filtro, cada filtro isolado, filtros combinados, inativos, IDs vazios, nome inexistente, segunda página e página além do fim; também busca por ID, inserção, alteração, desativação e todos os comandos dos vínculos. As gravações foram desfeitas com ROLLBACK.

Depois da implementação, uma aplicação temporária fora do repositório validou os nove métodos reais, inclusive o mapeamento das dez colunas, parâmetros com aspas, descrição NULL, preservação da data de cadastro, duplicidade de vínculo convertida pela base, remoção e regravação de gêneros e desativação preservando os relacionamentos. O programa de teste usou TransactionScope fora dos repositórios, sem Complete, e confirmou o desfazimento das gravações: 20 jogos, 34 vínculos e nenhum registro de teste. A solução compilou com zero erros e zero avisos. Não foi realizada execução no Workbench nem no MySQL 8.
