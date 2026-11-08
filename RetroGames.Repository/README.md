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
