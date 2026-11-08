-- Somente consultas: execute após 003 no mesmo schema.
-- Quantidades esperadas na carga nova: 3, 2, 6, 8, 20, 34 e 0.
SELECT COUNT(*) AS quantidade_condicoes FROM tb_condicao_jogo;
SELECT COUNT(*) AS quantidade_perfis FROM tb_perfil;
SELECT COUNT(*) AS quantidade_plataformas FROM tb_plataforma;
SELECT COUNT(*) AS quantidade_generos FROM tb_genero;
SELECT COUNT(*) AS quantidade_jogos FROM tb_jogo;
SELECT COUNT(*) AS quantidade_associacoes FROM tb_jogo_genero;
SELECT COUNT(*) AS quantidade_usuarios FROM tb_usuario;

-- Confira as chaves fixas e suas descrições.
SELECT co_condicao_jogo, ds_condicao_jogo
FROM tb_condicao_jogo ORDER BY co_condicao_jogo;
SELECT co_perfil, ds_perfil FROM tb_perfil ORDER BY co_perfil;

-- Esperado: 20 jogos distintos associados, cada um com 1 a 3 gêneros.
SELECT COUNT(DISTINCT co_jogo) AS quantidade_jogos_com_generos
FROM tb_jogo_genero;
SELECT co_jogo, COUNT(*) AS quantidade_generos
FROM tb_jogo_genero
GROUP BY co_jogo
ORDER BY co_jogo;

-- Esperado: nenhuma linha com quantidade fora da faixa.
SELECT co_jogo, COUNT(*) AS quantidade_generos
FROM tb_jogo_genero
GROUP BY co_jogo
HAVING COUNT(*) NOT BETWEEN 1 AND 3;

-- Esperado: Shenmue inativo e Ocarina of Time com estoque zero.
SELECT co_seq_jogo, no_jogo, st_ativo, qt_estoque
FROM tb_jogo WHERE st_ativo = FALSE ORDER BY co_seq_jogo;
SELECT co_seq_jogo, no_jogo, st_ativo, qt_estoque
FROM tb_jogo WHERE qt_estoque = 0 ORDER BY co_seq_jogo;
