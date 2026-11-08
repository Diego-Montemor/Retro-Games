-- Exercícios SQL (especificação 4.2 e 4.3). Execute após a carga 003.
-- Os JOINs são usados neste exercício direto no banco. No Repository da API,
-- cada consulta continuará lendo uma tabela, com composição no Service via LINQ.
-- Execução conferida no MariaDB 11.8.8 autorizado pelo estudante:
-- 1: 19 jogos ativos; 2: Metroid com Ação, Aventura e Plataforma;
-- 3: 7 jogos de Aventura, incluindo Shenmue inativo;
-- 4: 6 plataformas, totalizando 20 jogos;
-- 5: primeira e segunda páginas de 'o' com 5 jogos distintos em cada página.
-- O LEFT JOIN também foi conferido com Atari 2600 inserido temporariamente:
-- a plataforma apareceu com quantidade zero e foi desfeita com ROLLBACK.

-- 1. Catálogo: somente jogos ativos, com plataforma e condição.
SELECT jogo.co_seq_jogo, jogo.no_jogo, plataforma.no_plataforma,
       condicao.ds_condicao_jogo, jogo.vl_preco, jogo.qt_estoque
FROM tb_jogo AS jogo
INNER JOIN tb_plataforma AS plataforma
    ON plataforma.co_seq_plataforma = jogo.co_plataforma
INNER JOIN tb_condicao_jogo AS condicao
    ON condicao.co_condicao_jogo = jogo.co_condicao_jogo
WHERE jogo.st_ativo = TRUE
ORDER BY jogo.no_jogo, jogo.co_seq_jogo;

-- 2. Gêneros de um jogo informado. Altere o código para consultar outro jogo.
SET @codigo_jogo = (SELECT co_seq_jogo FROM tb_jogo
                   WHERE no_jogo = 'Metroid' ORDER BY co_seq_jogo LIMIT 1);
SELECT genero.co_seq_genero, genero.no_genero
FROM tb_genero AS genero
INNER JOIN tb_jogo_genero AS associacao
    ON associacao.co_genero = genero.co_seq_genero
WHERE associacao.co_jogo = @codigo_jogo
ORDER BY genero.no_genero, genero.co_seq_genero;

-- 3. Jogos de um gênero informado (inclui ativos e inativos).
SET @codigo_genero = (SELECT co_seq_genero FROM tb_genero WHERE no_genero = 'Aventura');
SELECT jogo.co_seq_jogo, jogo.no_jogo, jogo.st_ativo
FROM tb_jogo AS jogo
INNER JOIN tb_jogo_genero AS associacao
    ON associacao.co_jogo = jogo.co_seq_jogo
WHERE associacao.co_genero = @codigo_genero
ORDER BY jogo.no_jogo, jogo.co_seq_jogo;

-- 4. Total por plataforma, incluindo inativos e plataformas sem jogos.
-- LEFT JOIN preserva a plataforma sem jogo; COUNT da PK do jogo retorna zero.
SELECT plataforma.co_seq_plataforma, plataforma.no_plataforma,
       COUNT(jogo.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma AS plataforma
LEFT JOIN tb_jogo AS jogo ON jogo.co_plataforma = plataforma.co_seq_plataforma
GROUP BY plataforma.co_seq_plataforma, plataforma.no_plataforma
ORDER BY plataforma.no_plataforma, plataforma.co_seq_plataforma;

-- 5. Busca por parte do nome, incluindo inativos, com cinco itens por página.
-- Altere o trecho e a página (inteiro >= 1). A primeira página é 1.
SET @trecho_nome = 'o';
SET @pagina = 1;
SET @padrao_nome = CONCAT('%', @trecho_nome, '%');
SET @deslocamento = (@pagina - 1) * 5;
-- PREPARE permite parametrizar o OFFSET sem concatenar valores no SQL.
PREPARE consulta_busca FROM
    'SELECT co_seq_jogo, no_jogo, vl_preco, qt_estoque, st_ativo
     FROM tb_jogo
     WHERE no_jogo LIKE ?
     ORDER BY no_jogo, co_seq_jogo
     LIMIT 5 OFFSET ?';
EXECUTE consulta_busca USING @padrao_nome, @deslocamento;
DEALLOCATE PREPARE consulta_busca;
