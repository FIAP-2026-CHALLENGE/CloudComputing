# 🐾 PetCare API — Cloud Computing Sprint 1

API REST para gerenciamento de cuidados veterinários de animais, desenvolvida em .NET 10 com Oracle Database, containerizada via Docker e implantada em nuvem Azure.

---

## 📋 Descrição do Projeto

O **PetCare API** é um sistema de gestão de saúde animal que permite cadastrar responsáveles, animais e registrar eventos de cuidado veterinário (vacinas, consultas, exames, medicamentos etc.). A solução foi containerizada com Docker Compose e implantada em uma VM Linux na Azure via script CLI automatizado.

---

## 💼 Benefícios para o Negócio

- **Centralização de dados**: histórico completo de saúde de cada animal acessível em qualquer lugar
- **Rastreabilidade**: controle de vacinas, medicamentos e consultas com status e prioridade
- **Escalabilidade**: arquitetura em containers permite escalar horizontalmente conforme demanda
- **Redução de custos operacionais**: infraestrutura provisionada e destruída via script, sem desperdício de recursos em nuvem
- **Segurança**: aplicação rodando com usuário sem privilégios administrativos dentro do container

---

## 🏗️ Desenho Macro da Arquitetura

```
┌──────────────────────────────────────────────────────────────┐
│                        Azure Cloud                           │
│                                                              │
│   ┌──────────────────────────────────────────────────────┐   │
│   │              VM Linux Ubuntu 24.04                   │   │
│   │              (Standard_B2s — eastus)                 │   │
│   │                                                      │   │
│   │   ┌─────────────────────────────────────────────┐   │   │
│   │   │         Docker Network (bridge)             │   │   │
│   │   │                                             │   │   │
│   │   │  ┌───────────────┐    ┌──────────────────┐  │   │   │
│   │   │  │  cloudcomputing│    │cloudcomputing    │  │   │   │
│   │   │  │     -api       │───▶│   -oracle-db     │  │   │   │
│   │   │  │  (.NET 10)     │    │ (Oracle XE 21)   │  │   │   │
│   │   │  │  porta 8081    │    │  porta 1521      │  │   │   │
│   │   │  └───────────────┘    └────────┬─────────┘  │   │   │
│   │   │                                │             │   │   │
│   │   │                   ┌────────────▼──────────┐  │   │   │
│   │   │                   │  Volume Nomeado       │  │   │   │
│   │   │                   │ cloudcomputing-oracle  │  │   │   │
│   │   │                   │       -data            │  │   │   │
│   │   │                   └───────────────────────┘  │   │   │
│   │   └─────────────────────────────────────────────┘   │   │
│   │                                                      │   │
│   │   NSG: portas 22 (SSH), 8081 (API), 1521 (Oracle)   │   │
│   └──────────────────────────────────────────────────────┘   │
│                                                              │
└──────────────────────────────────────────────────────────────┘
              ▲
              │ HTTP :8081/swagger
              │
         👤 Usuário / Avaliador
```

---

## 🛣️ Rotas da API

### Responsaveis — `/api/responsaveis`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/responsaveis` | Lista todos os responsaveis |
| GET | `/api/responsaveis/{id}` | Busca responsável por ID |
| GET | `/api/responsaveis/cpf/{cpf}` | Busca responsável por CPF |
| POST | `/api/responsaveis` | Cadastra novo responsavel |
| PUT | `/api/responsaveis/{id}` | Atualiza responsável existente |
| DELETE | `/api/responsaveis/{id}` | Remove responsável |

### Animais — `/api/animais`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/animais` | Lista todos os animais |
| GET | `/api/animais/{id}` | Busca animal por ID |
| GET | `/api/animais/responsável/{responsavelId}` | Lista animais de um responsável |
| POST | `/api/animais` | Cadastra novo animal |
| PUT | `/api/animais/{id}` | Atualiza animal existente |
| DELETE | `/api/animais/{id}` | Remove animal |

### Care Events — `/api/care-events`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/care-events` | Lista todos os eventos |
| GET | `/api/care-events/{id}` | Busca evento por ID |
| GET | `/api/care-events/animal/{animalId}` | Eventos de um animal |
| GET | `/api/care-events/status/{status}` | Filtra por status |
| GET | `/api/care-events/type/{type}` | Filtra por tipo |
| GET | `/api/care-events/animal/{animalId}/status/{status}` | Filtra por animal e status |
| GET | `/api/care-events/overdue` | Lista eventos em atraso |
| POST | `/api/care-events` | Cadastra novo evento |
| PUT | `/api/care-events/{id}` | Atualiza evento |
| PATCH | `/api/care-events/{id}/complete` | Marca evento como concluído |
| DELETE | `/api/care-events/{id}` | Remove evento |

---

## 🚀 Instalação da Solução (How To)

### Pré-requisitos

- [Docker](https://docs.docker.com/get-docker/) e Docker Compose instalados
- Git instalado
- Conta Azure com CLI configurada (para deploy em nuvem)

### 1. Clonar o repositório

```bash
git clone https://github.com/SEU_USUARIO/SEU_REPOSITORIO.git
cd CloudComputing
```

### 2. Executar localmente com Docker Compose

```bash
# Sobe API + Oracle em background
docker compose up -d --build

# Acompanhar logs
docker compose logs -f

# Verificar containers rodando
docker compose ps
```

A API ficará disponível em: **http://localhost:8081/swagger**

> ⚠️ O Oracle XE leva entre 2 e 3 minutos para inicializar na primeira execução. A API aguarda automaticamente via `healthcheck` antes de subir.

### 3. Deploy em nuvem (Azure)

```bash
# Edite a variável GITHUB_REPO no script com a URL do seu repositório
nano azure-deploy.sh

# Execute o script de provisionamento
chmod +x azure-deploy.sh
./azure-deploy.sh
```

O script realiza automaticamente:
1. Login na Azure
2. Criação do Resource Group
3. Provisionamento da VM Linux Ubuntu 24.04
4. Abertura das portas necessárias no NSG (22, 8081, 1521)
5. Instalação do Docker e ferramentas na VM
6. Clone do repositório e start com `docker compose up -d`

Ao final, o terminal exibe o IP público e a URL do Swagger.

### 4. Destruir a infraestrutura após a apresentação

```bash
az group delete --name rg-cloudcomputing --yes --no-wait
```

---

## 🐳 Dockerfile

Multi-stage build: compila em `sdk:10.0` e executa em `aspnet:10.0` (imagem menor).  
A aplicação roda com o usuário `1654` (sem privilégios root).

## 🐙 Docker Compose

Dois serviços na mesma rede bridge `cloudcomputing-network`:

- **cloudcomputing-oracle-db**: Oracle XE 21 com volume nomeado para persistência
- **cloudcomputing-api**: API .NET com `depends_on` + `healthcheck` do Oracle

---

## ☁️ Script Azure CLI

Arquivo: [`azure-deploy.sh`](./azure-deploy.sh)

Provisiona toda a infraestrutura necessária em sequência, desde a VM até a execução da aplicação em background via Docker Compose.
