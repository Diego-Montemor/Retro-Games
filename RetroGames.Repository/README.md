# Base do Repository

`RepositoryConfig.StringConexao` guarda somente o texto de conexão. Seu valor será fornecido pela aplicação na etapa de configuração, via user-secrets; nenhuma credencial fica neste projeto.

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
