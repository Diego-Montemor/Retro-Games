-- Somente consultas: execute após selecionar o schema criado.
SELECT VERSION() AS versao_servidor, DATABASE() AS banco_selecionado;
SHOW TABLES;

-- Esperado: sete tabelas.
SELECT COUNT(*) AS quantidade_tabelas
FROM information_schema.tables
WHERE table_schema = DATABASE() AND table_type = 'BASE TABLE';

-- Colunas, tipos, obrigatoriedade, padrões e AUTO_INCREMENT.
SELECT table_name, ordinal_position, column_name, column_type,
       is_nullable, column_default, extra
FROM information_schema.columns
WHERE table_schema = DATABASE()
ORDER BY table_name, ordinal_position;

-- PKs compostas, UNIQUE e índices, incluindo ix_jogo_nome.
SELECT table_name, index_name, non_unique, seq_in_index, column_name
FROM information_schema.statistics
WHERE table_schema = DATABASE()
ORDER BY table_name, index_name, seq_in_index;

-- Esperado: cinco FKs, sem CASCADE/SET NULL.
SELECT table_name, constraint_name, referenced_table_name,
       update_rule, delete_rule
FROM information_schema.referential_constraints
WHERE constraint_schema = DATABASE()
ORDER BY table_name, constraint_name;

SHOW CREATE TABLE tb_jogo;
SHOW CREATE TABLE tb_jogo_genero;
