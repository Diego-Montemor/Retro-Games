-- Etapa 4: validar SQL antes de implementar JogoRepository e JogoGeneroRepository.
-- Execute no banco retrogames após a carga 003, na mesma conexão.
-- Testes de escrita são desfeitos com ROLLBACK. AUTO_INCREMENT pode avançar.
-- Executado antes da implementação no MariaDB 11.8.8 autorizado, pelo terminal.
-- Todos os resultados esperados abaixo foram confirmados, sem erros SQL.
-- Foram confirmadas também a descrição NULL, a preservação da data de cadastro,
-- a desativação sem remover jogo/vínculos e a remoção dos vínculos por jogo.
-- Estado final observado: 20 jogos, 34 vínculos e zero registros temporários.
SET @incluir_inativos = FALSE;
SET @tamanho_pagina = 5;
SET @deslocamento = 0;
SET @busca = '%Mario%';
SET @id_plataforma = 1;
SET @id_0 = 1;
SET @id_1 = 20;

-- Sem filtro: total 19, primeira página com 5 jogos.
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE);
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Segunda página: total 19, cinco jogos diferentes da primeira.
SET @deslocamento = 5;
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE);
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Nome Mario: total 4.
SET @deslocamento = 0;
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @busca;
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @busca ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Plataforma NES: total 4.
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND co_plataforma = @id_plataforma;
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND co_plataforma = @id_plataforma ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- IDs 1 e 20: total 1, porque Shenmue está inativo.
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND co_seq_jogo IN (@id_0, @id_1);
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND co_seq_jogo IN (@id_0, @id_1) ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- IDs 1 e 20 incluindo inativos: total 2.
SET @incluir_inativos = TRUE;
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND co_seq_jogo IN (@id_0, @id_1);
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND co_seq_jogo IN (@id_0, @id_1) ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Lista de IDs vazia: total zero.
SET @incluir_inativos = FALSE;
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND 1 = 0;
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND 1 = 0 ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Filtros combinados: Mario na NES entre IDs 1 e 20, total 1.
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @busca AND co_plataforma = @id_plataforma AND co_seq_jogo IN (@id_0, @id_1);
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @busca AND co_plataforma = @id_plataforma AND co_seq_jogo IN (@id_0, @id_1) ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Página além do fim: total 19, página vazia.
SET @deslocamento = 100;
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE);
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Nome inexistente: total zero.
SET @busca = '%Nao existe este jogo%';
SET @deslocamento = 0;
SELECT COUNT(*) AS total FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @busca;
PREPARE pagina_jogos FROM 'SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @busca ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE pagina_jogos USING @tamanho_pagina, @deslocamento;
DEALLOCATE PREPARE pagina_jogos;

-- Buscar jogo por ID: esperado Super Mario Bros. e descrição NULL.
SET @id = 1;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo = @id;
-- ID inexistente: esperado nenhuma linha.
SET @id = 2147483647;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo = @id;

-- Todos os vínculos: esperado 34 linhas.
SELECT co_jogo, co_genero FROM tb_jogo_genero ORDER BY co_jogo, co_genero;
-- Vínculos de jogos 1 e 3: esperado 4 linhas.
SET @id_0 = 1;
SET @id_1 = 3;
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (@id_0, @id_1) ORDER BY co_jogo, co_genero;

START TRANSACTION;
SET @nome = 'Jogo temporario da etapa 4';
SET @descricao = NULL;
SET @ano_lancamento = 1990;
SET @preco = 99.90;
SET @estoque = 2;
SET @ativo = TRUE;
SET @data_cadastro = UTC_TIMESTAMP();
SET @id_plataforma = 1;
SET @id_condicao_jogo = 2;

-- Inserir jogo e recuperar ID.
INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES (@nome, @descricao, @ano_lancamento, @preco, @estoque, @ativo, @data_cadastro, @id_plataforma, @id_condicao_jogo);
SET @id = LAST_INSERT_ID();
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo = @id;

-- Inserir vínculos em um único comando.
SET @id_jogo = @id;
SET @id_genero_0 = 1;
SET @id_genero_1 = 2;
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@id_jogo, @id_genero_0), (@id_jogo, @id_genero_1);
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (@id_jogo) ORDER BY co_jogo, co_genero;

-- Alterar dados, preservando dt_cadastro.
SET @nome = 'Jogo temporario alterado';
SET @descricao = 'Descricao alterada';
SET @ano_lancamento = 1991;
SET @preco = 109.90;
SET @estoque = 3;
SET @id_plataforma = 2;
SET @id_condicao_jogo = 3;
UPDATE tb_jogo SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @ano_lancamento, vl_preco = @preco, qt_estoque = @estoque, st_ativo = @ativo, co_plataforma = @id_plataforma, co_condicao_jogo = @id_condicao_jogo WHERE co_seq_jogo = @id;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo = @id;

-- Desativar: registro e vínculos continuam existindo.
UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo = @id;
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (@id_jogo) ORDER BY co_jogo, co_genero;

-- Remover vínculos por jogo: esperado nenhuma linha depois.
DELETE FROM tb_jogo_genero WHERE co_jogo = @id_jogo;
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (@id_jogo) ORDER BY co_jogo, co_genero;
ROLLBACK;

-- Esperado: 20 jogos, 34 vínculos e nenhum jogo temporário.
SELECT COUNT(*) AS quantidade_jogos FROM tb_jogo;
SELECT COUNT(*) AS quantidade_vinculos FROM tb_jogo_genero;
SELECT COUNT(*) AS quantidade_testes FROM tb_jogo WHERE no_jogo IN ('Jogo temporario da etapa 4', 'Jogo temporario alterado');
