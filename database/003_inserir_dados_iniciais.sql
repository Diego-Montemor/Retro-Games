-- RetroGames: carga inicial didática (especificação 4.2).
-- Execute depois de 001 e 002, no schema selecionado, com as tabelas vazias.
-- Execute o arquivo inteiro na mesma conexão para preservar as variáveis de sessão.
-- Execute somente uma vez: a carga não é idempotente. Interrompa se houver erro.
-- Jogos e consoles são reais. Preços, estoques e condições são fictícios.
-- Anos indicam a primeira edição do jogo para a plataforma informada.
-- Não cadastra usuários; o administrador será criado pela API.

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'), (2, 'Usado'), (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'), (2, 'Cliente');

-- IDs AUTO_INCREMENT são capturados sem presumir valores sequenciais.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('NES', 'Nintendo', 1983);
SET @plataforma_nes = LAST_INSERT_ID();

INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Super Nintendo', 'Nintendo', 1990);
SET @plataforma_super_nintendo = LAST_INSERT_ID();

INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Nintendo 64', 'Nintendo', 1996);
SET @plataforma_nintendo_64 = LAST_INSERT_ID();

INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Mega Drive', 'Sega', 1988);
SET @plataforma_mega_drive = LAST_INSERT_ID();

INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('PlayStation', 'Sony', 1994);
SET @plataforma_playstation = LAST_INSERT_ID();

INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Dreamcast', 'Sega', 1998);
SET @plataforma_dreamcast = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('Plataforma');
SET @genero_plataforma = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('Ação');
SET @genero_acao = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('Aventura');
SET @genero_aventura = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('RPG');
SET @genero_rpg = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('Corrida');
SET @genero_corrida = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('Luta');
SET @genero_luta = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('Tiro');
SET @genero_tiro = LAST_INSERT_ID();

INSERT INTO tb_genero (no_genero) VALUES ('Quebra-cabeça');
SET @genero_quebra_cabeca = LAST_INSERT_ID();

-- Super Mario Bros.
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Super Mario Bros.', 1985, 149.90, 5, TRUE,
        UTC_TIMESTAMP(), @plataforma_nes, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma);

-- The Legend of Zelda
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('The Legend of Zelda', 1986, 249.90, 3, TRUE,
        UTC_TIMESTAMP(), @plataforma_nes, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_aventura);

-- Metroid
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Metroid', 1986, 199.90, 2, TRUE,
        UTC_TIMESTAMP(), @plataforma_nes, 3);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma),
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_aventura);

-- Tetris
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Tetris', 1989, 99.90, 4, TRUE,
        UTC_TIMESTAMP(), @plataforma_nes, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_quebra_cabeca);

-- Super Mario World
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Super Mario World', 1990, 179.90, 6, TRUE,
        UTC_TIMESTAMP(), @plataforma_super_nintendo, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma);

-- Chrono Trigger
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Chrono Trigger', 1995, 499.90, 2, TRUE,
        UTC_TIMESTAMP(), @plataforma_super_nintendo, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_rpg);

-- Super Metroid
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Super Metroid', 1994, 349.90, 3, TRUE,
        UTC_TIMESTAMP(), @plataforma_super_nintendo, 3);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma),
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_aventura);

-- Super Mario Kart
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Super Mario Kart', 1992, 189.90, 4, TRUE,
        UTC_TIMESTAMP(), @plataforma_super_nintendo, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_corrida);

-- Super Mario 64
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Super Mario 64', 1996, 229.90, 5, TRUE,
        UTC_TIMESTAMP(), @plataforma_nintendo_64, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma),
    (@codigo_jogo, @genero_aventura);

-- The Legend of Zelda: Ocarina of Time
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('The Legend of Zelda: Ocarina of Time', 1998, 299.90, 0, TRUE,
        UTC_TIMESTAMP(), @plataforma_nintendo_64, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_aventura);

-- GoldenEye 007
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('GoldenEye 007', 1997, 219.90, 3, TRUE,
        UTC_TIMESTAMP(), @plataforma_nintendo_64, 3);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_tiro);

-- Sonic the Hedgehog
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Sonic the Hedgehog', 1991, 129.90, 7, TRUE,
        UTC_TIMESTAMP(), @plataforma_mega_drive, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma);

-- Streets of Rage 2
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Streets of Rage 2', 1992, 199.90, 2, TRUE,
        UTC_TIMESTAMP(), @plataforma_mega_drive, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_luta);

-- Gunstar Heroes
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Gunstar Heroes', 1993, 279.90, 2, TRUE,
        UTC_TIMESTAMP(), @plataforma_mega_drive, 3);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma),
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_tiro);

-- Final Fantasy VII
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Final Fantasy VII', 1997, 249.90, 4, TRUE,
        UTC_TIMESTAMP(), @plataforma_playstation, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_rpg);

-- Tekken 3
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Tekken 3', 1998, 119.90, 5, TRUE,
        UTC_TIMESTAMP(), @plataforma_playstation, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_luta);

-- Crash Bandicoot
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Crash Bandicoot', 1996, 149.90, 3, TRUE,
        UTC_TIMESTAMP(), @plataforma_playstation, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma);

-- Sonic Adventure
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Sonic Adventure', 1998, 199.90, 4, TRUE,
        UTC_TIMESTAMP(), @plataforma_dreamcast, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_plataforma),
    (@codigo_jogo, @genero_aventura);

-- Crazy Taxi
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Crazy Taxi', 2000, 159.90, 2, TRUE,
        UTC_TIMESTAMP(), @plataforma_dreamcast, 3);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_corrida),
    (@codigo_jogo, @genero_acao);

-- Shenmue
INSERT INTO tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Shenmue', 1999, 299.90, 1, FALSE,
        UTC_TIMESTAMP(), @plataforma_dreamcast, 2);
SET @codigo_jogo = LAST_INSERT_ID();
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (@codigo_jogo, @genero_acao),
    (@codigo_jogo, @genero_aventura);
