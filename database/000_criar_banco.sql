-- RetroGames: criação do banco em MySQL 8.
-- Execute uma única vez, antes de 001_criar_tabelas.sql.
-- Se o banco já existir, confira a estrutura antes de prosseguir.
-- O erro de banco existente é intencional para evitar reutilizá-lo sem conferir.

CREATE DATABASE retrogames
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE retrogames;
