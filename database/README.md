# Banco de dados RetroGames

## Scripts e ordem de execução

1. `000_criar_banco.sql`: cria o banco `retrogames` com charset `utf8mb4` e seleciona esse banco na conexão atual.
2. `001_criar_tabelas.sql`: cria as sete tabelas, suas chaves e índices no banco selecionado.
3. `002_conferir_estrutura.sql`: consulta a estrutura criada para conferência.

Execute nessa ordem em um servidor **MySQL 8**. Os scripts de criação são destinados a uma instalação nova e acusam erro se o banco ou as tabelas já existirem. Se ocorrer um erro, interrompa a execução e confira o destino antes de continuar. Nenhum script apaga dados.

No terminal, entre na pasta `Retro-Games` antes de abrir o cliente `mysql`. Depois de se conectar ao servidor, use, na mesma sessão:

```sql
SOURCE database/000_criar_banco.sql;
SOURCE database/001_criar_tabelas.sql;
SOURCE database/002_conferir_estrutura.sql;
```

O cliente deve ser iniciado a partir da pasta `Retro-Games`, pois `SOURCE` resolve os caminhos a partir dela. Execute cada comando somente se o anterior concluir sem erros. No DataGrip, siga os passos abaixo. Se executar os arquivos em conexões diferentes, selecione `retrogames` como schema de destino antes de executar 001 e 002.

Os scripts criam somente a estrutura. Os códigos de condições, perfis e demais dados iniciais precisam ser definidos antes de um script de carga; nenhum usuário ou senha é cadastrado por SQL.

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

Os arquivos foram conferidos contra o PDF, o diagrama e o checklist. Não foram executados em um servidor MySQL nesta tarefa; a listagem real de tabelas, a inspeção no DataGrip e o teste de recriação precisam ser confirmados após a execução.

Referências de operação do DataGrip:
- https://www.jetbrains.com/help/datagrip/mysql.html
- https://www.jetbrains.com/help/datagrip/run-sql-files.html
