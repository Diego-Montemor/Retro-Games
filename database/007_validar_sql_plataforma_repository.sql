-- Etapa 3: SQL validado antes da implementação de PlataformaRepository.
-- Execute após a carga inicial, inteiro na mesma conexão.
-- Os valores abaixo são exemplos para o cliente SQL; no C# serão parâmetros.
-- As gravações de teste são desfeitas; AUTO_INCREMENT pode avançar.
-- Resultado observado no MariaDB 11.8.8 autorizado, via cliente de terminal:
-- listar retornou seis plataformas ordenadas; buscar ID 1 retornou NES;
-- nome NES encontrado, mas não ao ignorar ID 1; NES possui jogos;
-- a plataforma inserida recebeu ID, não possuía jogos e foi alterada e excluída;
-- após ROLLBACK: seis plataformas, nenhum registro de teste.
SET @id = 1;
SET @nome = 'NES';
SET @id_ignorar = NULL;

-- 1. Listar: esperado seis plataformas ordenadas por nome.
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
ORDER BY no_plataforma;

-- 2. Buscar por ID: esperado NES.
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- 3. Existe com nome: uma linha significa verdadeiro; nenhuma, falso.
SELECT 1
FROM tb_plataforma
WHERE no_plataforma = @nome
  AND (@id_ignorar IS NULL OR co_seq_plataforma <> @id_ignorar)
LIMIT 1;

-- Ignorar a própria NES: esperado nenhuma linha.
SET @id_ignorar = @id;
SELECT 1
FROM tb_plataforma
WHERE no_plataforma = @nome
  AND (@id_ignorar IS NULL OR co_seq_plataforma <> @id_ignorar)
LIMIT 1;

-- 4. Possui jogos: esperado uma linha para NES.
SELECT 1
FROM tb_jogo
WHERE co_plataforma = @id
LIMIT 1;

START TRANSACTION;
SET @nome = 'Atari 2600';
SET @fabricante = 'Atari';
SET @ano_lancamento = 1977;

-- 5. Inserir: esperado uma linha inserida e um ID gerado.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES (@nome, @fabricante, @ano_lancamento);
SET @id = LAST_INSERT_ID();
SELECT @id AS id_gerado;

-- A plataforma recém-criada não possui jogos: esperado nenhuma linha.
SELECT 1
FROM tb_jogo
WHERE co_plataforma = @id
LIMIT 1;

-- 6. Alterar: esperado nome, fabricante e ano atualizados no SELECT seguinte.
SET @nome = 'Atari 2600 de teste';
SET @fabricante = 'Atari de teste';
SET @ano_lancamento = 1978;
UPDATE tb_plataforma
SET no_plataforma = @nome,
    no_fabricante = @fabricante,
    nu_ano_lancamento = @ano_lancamento
WHERE co_seq_plataforma = @id;
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- 7. Excluir: esperado nenhuma linha no SELECT seguinte.
DELETE FROM tb_plataforma
WHERE co_seq_plataforma = @id;
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

ROLLBACK;

-- Estado preservado: esperado seis plataformas e zero registros de teste.
SELECT COUNT(*) AS quantidade_plataformas FROM tb_plataforma;
SELECT COUNT(*) AS quantidade_plataformas_teste
FROM tb_plataforma WHERE no_plataforma IN ('Atari 2600', 'Atari 2600 de teste');
