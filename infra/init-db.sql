-- RubroX Database Init
-- Creates required databases and schemas for Identity and RubroX API

-- Identity database
CREATE DATABASE identity;
\connect identity;
CREATE SCHEMA IF NOT EXISTS identity;

-- RubroX database
\connect rubrox;
CREATE SCHEMA IF NOT EXISTS presupuesto;
CREATE SCHEMA IF NOT EXISTS auditoria;

-- pgvector extension (for future semantic search)
CREATE EXTENSION IF NOT EXISTS vector;
