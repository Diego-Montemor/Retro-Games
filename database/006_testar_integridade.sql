-- Testes diretos no banco após a carga 003. Não são regras do Repository.
-- Execute o arquivo inteiro na mesma conexão, continuando após os erros esperados.
-- No cliente mysql/mariadb, use --force para continuar até o ROLLBACK.
-- No DataGrip, execute cada teste e finalize obrigatoriamente com ROLLBACK.
-- As alterações são desfeitas; contadores AUTO_INCREMENT podem avançar.
-- Executado no MariaDB 11.8.8, autorizado pelo estudante, com modo estrito ativo.
-- Ao final: 20 jogos, 6 plataformas, 34 associações e nenhum usuário.

START TRANSACTION;
SET @plataforma_valida = (SELECT MIN(co_seq_plataforma) FROM tb_plataforma);
SET @plataforma_inexistente = (SELECT MAX(co_seq_plataforma) + 1 FROM tb_plataforma);
SET @condicao_valida = (SELECT MIN(co_condicao_jogo) FROM tb_condicao_jogo);
SET @plataforma_com_jogos = (SELECT co_plataforma FROM tb_jogo ORDER BY co_seq_jogo LIMIT 1);

-- 1. Plataforma inexistente. Esperado: erro 1452 (FK).
-- Resultado observado: ERROR 1452 (23000), restrição fk_jogo_plataforma. Aprovado.
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste de plataforma inexistente', 1990, 10.00, 1, TRUE,
        UTC_TIMESTAMP(), @plataforma_inexistente, @condicao_valida);

-- 2. Nome de plataforma duplicado. Esperado: erro 1062 (UNIQUE).
-- Resultado observado: ERROR 1062 (23000), nome NES, chave uq_plataforma_nome. Aprovado.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
SELECT no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma WHERE co_seq_plataforma = @plataforma_valida;

-- 3. Exclusão de plataforma com jogos. Esperado: erro 1451 (FK).
-- Resultado observado: ERROR 1451 (23000), restrição fk_jogo_plataforma. Aprovado.
DELETE FROM tb_plataforma WHERE co_seq_plataforma = @plataforma_com_jogos;

-- 4. Nome de jogo nulo. Esperado: erro 1048 (NOT NULL).
-- Resultado observado: ERROR 1048 (23000), coluna no_jogo não aceita NULL. Aprovado.
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES (NULL, 1990, 10.00, 1, TRUE,
        UTC_TIMESTAMP(), @plataforma_valida, @condicao_valida);

-- 5. Preço zero. Esperado: aceito pelo banco; RN06 será validada no Service.
-- Resultado observado: INSERT aceito, SELECT retornou vl_preco = 0.00. Aprovado.
-- Após ROLLBACK, a consulta final retornou zero: o jogo de teste foi desfeito.
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste de preco zero - desfazer', 1990, 0.00, 1, TRUE,
        UTC_TIMESTAMP(), @plataforma_valida, @condicao_valida);
SET @jogo_preco_zero = LAST_INSERT_ID();
SELECT co_seq_jogo, no_jogo, vl_preco
FROM tb_jogo WHERE co_seq_jogo = @jogo_preco_zero;

ROLLBACK;

-- Esperado: zero. Confirma que o jogo de teste não ficou salvo.
SELECT COUNT(*) AS quantidade_jogos_teste_apos_rollback
FROM tb_jogo WHERE co_seq_jogo = @jogo_preco_zero;
