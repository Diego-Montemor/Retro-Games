# Banco de dados RetroGames

## Scripts e ordem de execução

1. `000_criar_banco.sql`: cria o banco `retrogames` com charset `utf8mb4` e seleciona esse banco na conexão atual.
2. `001_criar_tabelas.sql`: cria as sete tabelas, suas chaves e índices no banco selecionado.
3. `002_conferir_estrutura.sql`: consulta a estrutura criada para conferência.
4. `003_inserir_dados_iniciais.sql`: insere condições, perfis, plataformas, gêneros, jogos e suas associações.
5. `004_conferir_dados_iniciais.sql`: confere quantidades, chaves fixas e os casos de jogo inativo e estoque zero.
6. `005_consultas_catalogo.sql`: exercícios de catálogo, gêneros, totais por plataforma e busca paginada.
7. `006_testar_integridade.sql`: testes de FK, UNIQUE, NOT NULL e preço zero, desfeitos com `ROLLBACK`.

Execute nessa ordem em um servidor **MySQL 8**. Os scripts de criação são destinados a uma instalação nova e acusam erro se o banco ou as tabelas já existirem. Se ocorrer um erro, interrompa a execução e confira o destino antes de continuar. Nenhum script apaga dados.

No terminal, entre na pasta `Retro-Games` antes de abrir o cliente `mysql`. Depois de se conectar ao servidor, use, na mesma sessão:

```sql
SOURCE database/000_criar_banco.sql;
SOURCE database/001_criar_tabelas.sql;
SOURCE database/002_conferir_estrutura.sql;
SOURCE database/003_inserir_dados_iniciais.sql;
SOURCE database/004_conferir_dados_iniciais.sql;
```

O cliente deve ser iniciado a partir da pasta `Retro-Games`, pois `SOURCE` resolve os caminhos a partir dela. Execute cada comando somente se o anterior concluir sem erros. No DataGrip, siga os passos abaixo. Se executar os arquivos em conexões diferentes, selecione `retrogames` como schema de destino antes de executar 001 e 002.

### Carga inicial

Após os scripts da etapa de estrutura, execute 003 e 004 no mesmo schema. A carga 003 deve ser executada uma única vez, com as tabelas vazias, e por inteiro na mesma conexão: `LAST_INSERT_ID()` captura as chaves geradas em variáveis de sessão para montar os relacionamentos. Não se presume que os IDs gerados comecem em 1 ou sejam consecutivos. As únicas chaves fixas são as condições (1 Novo, 2 Usado, 3 Recondicionado) e os perfis (1 Administrador, 2 Cliente).

São inseridas 6 plataformas, 8 gêneros, 20 jogos e 34 associações, com 1 a 3 gêneros por jogo. Os jogos e consoles são reais; anos de lançamento correspondem à primeira edição na plataforma indicada, podendo variar por região. Preços, estoques e condições são fictícios para uso didático. `Shenmue` fica inativo e `The Legend of Zelda: Ocarina of Time` fica ativo com estoque zero. Datas de cadastro usam `UTC_TIMESTAMP()`.

Nenhum usuário é inserido: `tb_usuario` permanece vazia até a criação do administrador pela API na aula de autenticação. Se houver erro durante a carga, interrompa e confira o que já foi inserido antes de repetir: os comandos não formam uma carga idempotente nem desfazem inserções anteriores automaticamente.

O script 004 deve mostrar 3 condições, 2 perfis, 6 plataformas, 8 gêneros, 20 jogos, 34 associações e 0 usuários. Ele também lista a quantidade de gêneros por jogo e os casos especiais. No DataGrip, execute 003 e depois 004 após o passo 8 abaixo.

### Consultas e testes de integridade

Depois da carga, execute `SOURCE database/005_consultas_catalogo.sql;` no cliente conectado ao schema `retrogames`. No arquivo, altere `@codigo_jogo`, `@codigo_genero`, `@trecho_nome` e `@pagina` para consultar outros valores. Somente o catálogo filtra jogos ativos; as outras consultas incluem inativos. A ordenação da busca usa nome e código para manter as páginas estáveis. `%` e `_` no trecho de busca têm o significado de curingas do `LIKE`.

Os `JOINs` do arquivo 005 pertencem aos exercícios SQL diretos no banco. No código do Repository continua valendo uma tabela por consulta e composição com LINQ no Service.

Para executar `006_testar_integridade.sql`, abra o cliente `mysql` ou `mariadb` com a opção `--force` e depois use `SOURCE database/006_testar_integridade.sql;`. A opção permite continuar após os quatro erros intencionais e alcançar o `ROLLBACK`. No DataGrip, execute cada teste na mesma conexão e finalize com `ROLLBACK`. Não execute o arquivo como migração automática. A transação existe apenas para desfazer os dados de teste; os contadores `AUTO_INCREMENT` podem avançar.

Os cinco testes foram executados no MariaDB 11.8.8 autorizado e produziram os resultados esperados: erros 1452, 1062, 1451 e 1048, e aceitação de preço zero. O resultado de cada teste está comentado junto ao comando no arquivo 006. O preço zero confirma que RN06 deve ficar no Service. O `ROLLBACK` foi confirmado, mantendo 20 jogos e 6 plataformas. As consultas 005 também foram executadas, incluindo conferência da segunda página e de uma plataforma sem jogos inserida temporariamente e desfeita com `ROLLBACK`.

O script `001_criar_tabelas.sql` implementa as sete tabelas das seções 4.1–4.3 do PDF, usando os tamanhos de campos da imagem `retrogames-modelo-relacional.png`. As tabelas de Compras da imagem pertencem à tarefa futura e não entram neste script.

`002_conferir_estrutura.sql` contém apenas consultas para conferir colunas, índices e relacionamentos. Não é uma migração de estrutura.

Somente `ds_jogo` aceita NULL. Chaves `co_seq_` são geradas; condições e perfis usam códigos fixos. As cinco colunas indicadas no checklist possuem UNIQUE, e `no_jogo` possui índice não exclusivo. A associação jogo/gênero possui PK composta. As cinco FKs usam o comportamento padrão, sem ações referenciais explícitas.

`BOOLEAN` aparece como `tinyint(1)` e `TRUE` como `1` na inspeção do MySQL; são representações equivalentes. `dt_cadastro` não tem padrão automático: a aplicação deverá informar a data em UTC. As regras de preço, estoque, ano e exclusão lógica ficam no Service, sem CHECK, trigger ou function no banco.

## Conectar e criar pelo DataGrip

O DataGrip é um cliente: é necessário ter um servidor **MySQL 8** instalado, em execução e acessível. Ter o comando `mysql` instalado não comprova que exista um servidor ativo.

1. Abra o DataGrip e escolha **File → New → Data Source → MySQL**.
2. Informe o host e a porta do seu servidor (em uma instalação local padrão, `localhost` e `3306`). Informe seu usuário e senha somente na janela de conexão; não coloque credenciais nos scripts ou no repositório. Inicialmente deixe Database vazio se o banco ainda não existe.
3. Se aparecer **Download missing driver files**, baixe o driver. Clique em **Test Connection** e, se funcionar, em **OK**. Caso falhe, confira o servidor, host, porta e permissões antes de continuar.
4. Abra `000_criar_banco.sql` em um console associado à conexão e execute o arquivo inteiro.

   Esse comando exige permissão de criação e um nome ainda livre. Se já houver um banco com esse nome, use outro nome para esta validação ou confira se ele realmente está vazio; não apague um banco com dados para começar.
5. Nas propriedades da conexão, aba **Schemas**, marque `retrogames`, aplique e atualize o **Database Explorer**.
6. Abra `001_criar_tabelas.sql` pelo DataGrip. Associe o arquivo à conexão MySQL e selecione `retrogames` como schema de destino. Para executar o arquivo inteiro, use **SQL Scripts → Run SQL Script…** no menu de contexto do schema e escolha o arquivo. Confirme o destino antes de executar. O script não contém CREATE DATABASE nem USE para permitir também um schema de teste.
7. Atualize o schema no Database Explorer e expanda **Tables**. Devem aparecer `tb_condicao_jogo`, `tb_perfil`, `tb_plataforma`, `tb_genero`, `tb_jogo`, `tb_jogo_genero` e `tb_usuario`.
8. Execute `002_conferir_estrutura.sql` na mesma conexão e no mesmo schema. Confira sete tabelas, cinco FKs e apenas `ds_jogo` anulável. Compare a estrutura de `tb_jogo` com o diagrama usando a árvore de colunas/chaves/índices do DataGrip e o resultado de `SHOW CREATE TABLE tb_jogo` (equivalente à inspeção solicitada no Workbench).

## Validar a recriação sem afetar o banco principal

Crie um schema descartável chamado `retrogames_validacao` com o mesmo charset e collation. Selecione-o como destino, rode os scripts 001 e 002 e confira os resultados. Somente depois de confirmar que esse schema é o descartável criado por você, remova-o e crie-o novamente:

```sql
DROP DATABASE retrogames_validacao;
CREATE DATABASE retrogames_validacao
    CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
```

Atualize os schemas no DataGrip e execute 001 e 002 novamente nesse destino. O script 001 foi feito para banco vazio; executá-lo duas vezes sem remover as tabelas gera erro de tabela existente, intencionalmente, em vez de esconder diferenças estruturais com IF NOT EXISTS.

## Estado da verificação

Os arquivos foram conferidos contra o PDF, o diagrama e o checklist. A consulta 002 foi executada no banco local `retrogames`, confirmando sete tabelas, cinco FKs e somente `ds_jogo` anulável. A inspeção visual no DataGrip e o teste de recriação não foram realizados nesta execução.

Com autorização expressa do estudante para usar o servidor disponível, a carga 003 e as consultas 004 foram executadas sem erros no **MariaDB 11.8.8**, com o usuário `dev`. As tabelas estavam vazias antes da carga. Foram confirmadas 3 condições, 2 perfis, 6 plataformas, 8 gêneros, 20 jogos, 34 associações e 0 usuários. Todos os 20 jogos têm de 1 a 3 gêneros; `Shenmue` está inativo e `The Legend of Zelda: Ocarina of Time` tem estoque zero. A sessão usou `utf8mb4` para preservar os acentos. Nenhuma senha foi gravada nos arquivos.

A arquitetura continua especificando **MySQL 8**; a execução no MariaDB não comprova validação nessa versão do MySQL. Os scripts 000 e 001 não foram reexecutados, pois o banco e as sete tabelas já existiam.

Referências de operação do DataGrip:
- https://www.jetbrains.com/help/datagrip/mysql.html
- https://www.jetbrains.com/help/datagrip/run-sql-files.html
