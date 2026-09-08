-- =============================================================================
-- script_bd.sql
-- DDL das tabelas do banco de dados PetCare API (MySQL 8.0)
-- Gerado a partir das migrations do Entity Framework Core (Pomelo.EntityFrameworkCore.MySql)
-- =============================================================================

-- Tabela CORE: representa a pessoa responsável pelo(s) animal(is) cadastrado(s).
CREATE TABLE T_CP_RESPONSAVEIS (
    ID          INT NOT NULL AUTO_INCREMENT COMMENT 'Identificador único do responsável',
    NAME        VARCHAR(120) NOT NULL       COMMENT 'Nome completo do responsável',
    EMAIL       VARCHAR(160) NOT NULL       COMMENT 'E-mail de contato do responsável',
    PHONE       VARCHAR(20)  NOT NULL       COMMENT 'Telefone de contato do responsável',
    CPF         VARCHAR(14)  NOT NULL       COMMENT 'CPF do responsável (formato 000.000.000-00)',
    CREATED_AT  DATETIME(6)  NOT NULL       COMMENT 'Data/hora de criação do registro (UTC)',
    IS_ACTIVE   TINYINT(1)   NOT NULL       COMMENT 'Indica se o responsável está ativo no sistema',
    CONSTRAINT PK_T_CP_RESPONSAVEIS PRIMARY KEY (ID)
) ENGINE=InnoDB CHARACTER SET=utf8mb4
COMMENT='Responsáveis (tutores) pelos animais cadastrados na aplicação';


-- Tabela CORE: representa o animal cadastrado, vinculado a um responsável.
CREATE TABLE T_CP_ANIMAIS (
    ID              INT NOT NULL AUTO_INCREMENT COMMENT 'Identificador único do animal',
    RESPONSAVEL_ID  INT NOT NULL               COMMENT 'FK para T_CP_RESPONSAVEIS.ID (dono do animal)',
    NAME            VARCHAR(120) NOT NULL      COMMENT 'Nome do animal',
    NICKNAME        VARCHAR(120)               COMMENT 'Apelido do animal (opcional)',
    SPECIES         VARCHAR(30)  NOT NULL      COMMENT 'Espécie: DOG ou CAT',
    BREED           VARCHAR(80)  NOT NULL      COMMENT 'Raça do animal',
    BIRTH_DATE      DATETIME(6)  NOT NULL      COMMENT 'Data de nascimento do animal',
    WEIGHT          DECIMAL(10,2) NOT NULL     COMMENT 'Peso do animal em quilogramas',
    SEX             VARCHAR(20)  NOT NULL      COMMENT 'Sexo do animal: MALE ou FEMALE',
    RGA             VARCHAR(30)                COMMENT 'Registro Geral do Animal (opcional)',
    CREATED_AT      DATETIME(6)  NOT NULL      COMMENT 'Data/hora de criação do registro (UTC)',
    IS_ACTIVE       TINYINT(1)   NOT NULL      COMMENT 'Indica se o animal está ativo no sistema',
    CONSTRAINT PK_T_CP_ANIMAIS PRIMARY KEY (ID),
    CONSTRAINT FK_T_CP_ANIMAIS_T_CP_RESPONSAVEIS_RESPONSAVEL_ID
        FOREIGN KEY (RESPONSAVEL_ID) REFERENCES T_CP_RESPONSAVEIS (ID) ON DELETE RESTRICT
) ENGINE=InnoDB CHARACTER SET=utf8mb4
COMMENT='Animais cadastrados, vinculados a um responsável (tutor)';

CREATE INDEX IX_T_CP_ANIMAIS_RESPONSAVEL_ID ON T_CP_ANIMAIS (RESPONSAVEL_ID);


-- Tabela CORE: representa um evento de cuidado veterinário (vacina, consulta, exame, etc.)
-- vinculado a um animal.
CREATE TABLE T_CP_CARE_EVENTS (
    ID              INT NOT NULL AUTO_INCREMENT COMMENT 'Identificador único do evento de cuidado',
    PET_ID          INT NOT NULL               COMMENT 'FK para T_CP_ANIMAIS.ID (animal atendido)',
    TYPE            VARCHAR(40)  NOT NULL      COMMENT 'Tipo do evento: VACCINE, DEWORMING, MEDICATION, CHECKUP, RETURN, EXAM, GROOMING, SURGERY, OTHER',
    TITLE           VARCHAR(160) NOT NULL      COMMENT 'Título/resumo do evento',
    DESCRIPTION     VARCHAR(500)               COMMENT 'Descrição detalhada do evento (opcional)',
    SCHEDULED_DATE  DATETIME(6)  NOT NULL      COMMENT 'Data/hora agendada para o evento',
    COMPLETED_DATE  DATETIME(6)                COMMENT 'Data/hora de conclusão do evento (nulo se ainda pendente)',
    STATUS          VARCHAR(30)  NOT NULL      COMMENT 'Status: PENDING, COMPLETED, OVERDUE ou CANCELED',
    PRIORITY        VARCHAR(30)  NOT NULL      COMMENT 'Prioridade: LOW, MEDIUM, HIGH ou CRITICAL',
    NOTES           VARCHAR(500)               COMMENT 'Observações adicionais (opcional)',
    CREATED_AT      DATETIME(6)  NOT NULL      COMMENT 'Data/hora de criação do registro (UTC)',
    IS_ACTIVE       TINYINT(1)   NOT NULL      COMMENT 'Indica se o evento está ativo no sistema',
    CONSTRAINT PK_T_CP_CARE_EVENTS PRIMARY KEY (ID),
    CONSTRAINT FK_T_CP_CARE_EVENTS_T_CP_ANIMAIS_PET_ID
        FOREIGN KEY (PET_ID) REFERENCES T_CP_ANIMAIS (ID) ON DELETE RESTRICT
) ENGINE=InnoDB CHARACTER SET=utf8mb4
COMMENT='Eventos de cuidado veterinário (vacinas, consultas, exames, medicações etc.) associados a um animal';

CREATE INDEX IX_T_CP_CARE_EVENTS_PET_ID ON T_CP_CARE_EVENTS (PET_ID);


-- =============================================================================
-- Massa de dados inicial (seed) — inserida automaticamente pela aplicação no
-- startup (ver Program.cs), reproduzida aqui apenas como referência do
-- conteúdo mínimo exigido (2+ linhas significativas por tabela).
-- =============================================================================

-- INSERT INTO T_CP_RESPONSAVEIS (NAME, EMAIL, PHONE, CPF, CREATED_AT, IS_ACTIVE) VALUES
--   ('Ana Paula Souza', 'ana.souza@email.com', '(11) 91234-5678', '123.456.789-00', NOW(), 1),
--   ('Carlos Eduardo Lima', 'carlos.lima@email.com', '(21) 99876-5432', '987.654.321-00', NOW(), 1);

-- INSERT INTO T_CP_ANIMAIS (RESPONSAVEL_ID, NAME, NICKNAME, SPECIES, BREED, BIRTH_DATE, WEIGHT, SEX, RGA, CREATED_AT, IS_ACTIVE) VALUES
--   (1, 'Bolinha', 'Boli', 'DOG', 'Labrador', '2020-05-20', 28.5, 'MALE', 'RGA-001', NOW(), 1),
--   (2, 'Mia', 'Miau', 'CAT', 'Siamese', '2021-08-03', 4.2, 'FEMALE', 'RGA-002', NOW(), 1);

-- INSERT INTO T_CP_CARE_EVENTS (PET_ID, TYPE, TITLE, DESCRIPTION, SCHEDULED_DATE, COMPLETED_DATE, STATUS, PRIORITY, NOTES, CREATED_AT, IS_ACTIVE) VALUES
--   (1, 'VACCINE', 'Vacina V10 anual', 'Aplicacao anual da vacina polivalente V10.', '2025-06-01 09:00:00', '2025-06-01 09:30:00', 'COMPLETED', 'HIGH', 'Bolinha reagiu bem.', NOW(), 1),
--   (2, 'CHECKUP', 'Check-up semestral Mia', 'Consulta de rotina com exames de sangue e urina.', '2025-07-15 14:00:00', NULL, 'PENDING', 'MEDIUM', 'Agendar jejum de 8h antes.', NOW(), 1);
