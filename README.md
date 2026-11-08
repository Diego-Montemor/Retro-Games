# RetroGames

Estrutura inicial em .NET 10 conforme as seções 2, 3.1 e 3.2 da especificação.

| Projeto | Tipo | Referências diretas |
| --- | --- | --- |
| RetroGames.Model | Biblioteca de classes | Nenhuma |
| RetroGames.Repository | Biblioteca de classes | Model |
| RetroGames.Services | Biblioteca de classes | Model, Repository, WebApi.ApiModels |
| RetroGames.WebApi.ApiModels | Biblioteca de classes | Nenhuma |
| RetroGames.WebApi | API com controllers | Model, Repository, Services, WebApi.ApiModels |
| RetroGames.WebApi.UnitTests | xUnit v3 | WebApi, Services |
| retrogames.web | React 19, Vite 8 e JavaScript | API via HTTP |

O fluxo da aplicação será Controller → Service → Repository → banco. As referências de projetos seguem a tabela da seção 3.2; controllers não devem acessar repositórios diretamente.

Com o .NET SDK 10 instalado, execute nesta pasta:

```bash
dotnet restore RetroGames.sln
dotnet build RetroGames.sln
dotnet test RetroGames.sln
dotnet run --project RetroGames.WebApi
```

A API usa `http://localhost:5080`. Controllers estão habilitados, mas ainda não há endpoints de negócio. O documento OpenAPI fica em `/openapi/v1.json` somente em desenvolvimento.

As bibliotecas e o projeto de testes começam vazios. Entidades, regras e testes serão acrescentados nas etapas seguintes.

## Banco de dados

Os scripts para MySQL 8 ficam em `database/`. Execute `000_criar_banco.sql`, `001_criar_tabelas.sql` e `002_conferir_estrutura.sql`, nessa ordem, para criar o banco `retrogames`, criar as sete tabelas e conferir a estrutura. Veja as instruções de conexão e execução em [database/README.md](database/README.md).

Logo depois, execute `003_inserir_dados_iniciais.sql` e `004_conferir_dados_iniciais.sql` para carregar e conferir condições, perfis, 6 plataformas, 8 gêneros e 20 jogos. Usuários serão criados pela API na etapa de autenticação.

## Front-end

Com Node.js 22 LTS instalado, execute em outro terminal:

```bash
cd retrogames.web
npm install
npm run dev
```

A aplicação usa `http://localhost:5173`. O arquivo `vite.config.js` encaminha as requisições com prefixo `/api` para `http://localhost:5080`, mantendo a mesma origem no navegador.

`src/main.jsx` monta a aplicação React e `src/App.jsx` contém a página inicial. O front-end é um projeto independente da solução .NET.

Para verificar o build de produção, execute `npm run build` em `retrogames.web`. O resultado fica em `dist/`, ignorado pelo Git.
