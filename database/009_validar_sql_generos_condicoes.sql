-- Etapa 5: validar o SQL antes de criar os Repositories de leitura.
-- Execute após a carga 003 no banco retrogames. Somente consultas.
-- Executado antes da implementação, via terminal no MariaDB 11.8.8 autorizado.
-- Resultado confirmado: oito gêneros ordenados e as três condições esperadas.

-- Gêneros: esperado oito registros ordenados por nome.
SELECT co_seq_genero, no_genero
FROM tb_genero
ORDER BY no_genero;

-- Condições: esperado 1 Novo, 2 Usado, 3 Recondicionado.
SELECT co_condicao_jogo, ds_condicao_jogo
FROM tb_condicao_jogo
ORDER BY co_condicao_jogo;
