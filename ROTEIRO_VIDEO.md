# 🎬 ROTEIRO DO VÍDEO — Entrega 1º Sprint
# DevOps Tools & Cloud Computing — FIAP
# Duração estimada: 8 a 12 minutos
# Qualidade mínima: 720p | Narração por voz obrigatória em TODAS as partes
# ============================================================

## ─────────────────────────────────────────────
## PARTE 1 — SCRIPT AZURE CLI (Tarefa 01) ~3 min
## ─────────────────────────────────────────────

### [CENA 1] Tela do terminal aberta, projeto clonado na pasta

FALA:
"Olá, meu nome é [SEU NOME], RM [SEU RM], e vou demonstrar a entrega
do 1º Sprint da disciplina DevOps Tools & Cloud Computing.
Começo mostrando o script Azure CLI que provisiona toda a infraestrutura
necessária para rodar o projeto em nuvem."

AÇÃO:
- Abrir o arquivo azure-deploy.sh no editor (VS Code ou nano)
- Rolar devagar pelo arquivo mostrando as 7 seções comentadas:
  * Variáveis (Resource Group, VM, NSG)
  * az login
  * az group create
  * az vm create
  * az network nsg rule create (portas 22, 8081, 1521)
  * az vm run-command (instalação Docker + Git)
  * az vm run-command (clone do repo + docker compose up)

FALA:
"O script é dividido em 7 etapas sequenciais. Primeiro define as variáveis,
depois faz login na Azure, cria o Resource Group, provisiona a VM Ubuntu 24.04,
abre as portas necessárias no NSG — SSH na 22, a API na 8081 e o Oracle na 1521 —
instala o Docker, o Git e o nano diretamente na VM via run-command,
e por fim clona o repositório e sobe a aplicação com docker compose up em background."

### [CENA 2] Executar o script ao vivo

AÇÃO:
- No terminal: chmod +x azure-deploy.sh && ./azure-deploy.sh
- Deixar rodar mostrando cada etapa sendo executada
- Quando terminar, mostrar o resumo final com o IP público impresso no terminal

FALA:
"Vou executar o script agora. Cada etapa é exibida numerada no terminal.
[enquanto roda] Aqui o Resource Group sendo criado... a VM sendo provisionada...
as portas abrindo no NSG... o Docker sendo instalado...
e aqui o docker compose subindo em background na VM.
Ao final, o script imprime o IP público e a URL do Swagger onde a aplicação
já está acessível externamente."

## ─────────────────────────────────────────────
## PARTE 2 — APLICAÇÃO DOCKER NA NUVEM (Tarefa 02) ~5 min
## ─────────────────────────────────────────────

### [CENA 3] Mostrar docker-compose.yml e Dockerfile

AÇÃO:
- Abrir docker-compose.yml no editor
- Destacar: dois serviços, rede bridge, volume nomeado, depends_on + healthcheck
- Abrir Dockerfile
- Destacar: multi-stage build, USER 1654 (non-root), EXPOSE 8080

FALA:
"Agora mostro a configuração Docker. No docker-compose.yml temos dois serviços:
o banco Oracle XE na porta 1521, e a API .NET na porta 8081.
O Oracle usa um volume nomeado 'cloudcomputing-oracle-data' para persistência dos dados.
A API só sobe depois que o Oracle passa no healthcheck — evitando erros de conexão
na inicialização.
No Dockerfile usamos multi-stage build: compilamos no SDK e executamos na imagem
runtime menor do aspnet. Importante: o container roda com o usuário 1654,
sem privilégios root — requisito da disciplina."

### [CENA 4] Acessar o Swagger da aplicação rodando na nuvem

AÇÃO:
- No navegador: abrir http://[IP_PUBLICO]:8081/swagger
- Mostrar as 3 seções: Responsaveis, Animais, CareEvents
- Expandir alguns endpoints para mostrar os parâmetros

FALA:
"Aqui está a aplicação rodando em nuvem. Acesso o Swagger pelo IP público
da VM na porta 8081. Temos três grupos de rotas: Responsaveis para cadastro de responsáveles,
Animais para os animais, e CareEvents para os eventos de cuidado veterinário.
Cada entidade tem as operações GET, POST, PUT e DELETE completas."

### [CENA 5] Demonstrar CRUD — mostrar dados seedados e executar operações

AÇÃO SEQUENCIAL NO SWAGGER (mostrar cada uma claramente):

1. GET /api/responsávels — mostrar os 2 responsáveles seedados (Ana Paula, Carlos Eduardo)
2. GET /api/animais — mostrar os 2 animais seedados (Bolinha, Mia)
3. GET /api/care-events — mostrar os 2 eventos seedados (Vacina V10, Check-up)

FALA ao mostrar o GET inicial:
"Ao subir, a aplicação aplica automaticamente as migrations do EF Core e
popula o banco com dados iniciais. Vemos aqui os responsáveles, animais e eventos
de cuidado já inseridos — são os inserts significativos exigidos pelo enunciado."

4. POST /api/responsávels — criar novo responsável:
```json
{
  "name": "Fernanda Costa",
  "email": "fernanda.costa@email.com",
  "phone": "(11) 95555-1234",
  "cpf": "111.222.333-44"
}
```
FALA: "Crio um novo responsável via POST. Retorno 201 com o objeto criado."

5. POST /api/animais — criar novo animal (responsavelId do responsável recém criado):
```json
{
  "responsavelId": 3,
  "name": "Rex",
  "nickname": "Rexinho",
  "species": "DOG",
  "breed": "Golden Retriever",
  "birthDate": "2022-03-10T00:00:00Z",
  "weight": 32.0,
  "sex": "MALE",
  "rga": "RGA-003"
}
```
FALA: "Cadastro o animal Rex vinculado ao responsável recém criado."

6. POST /api/care-events — criar evento:
```json
{
  "animalId": 3,
  "type": "VACCINE",
  "title": "Vacina antirrábica",
  "description": "Aplicação anual da vacina antirrábica.",
  "scheduledDate": "2025-08-20T10:00:00Z",
  "status": "PENDING",
  "priority": "HIGH",
  "notes": "Levar carteira de vacinação."
}
```
FALA: "Registro um evento de vacinação para o Rex."

7. GET /api/care-events — listar tudo, mostrar o novo evento aparecendo
8. PATCH /api/care-events/3/complete — marcar como concluído
9. GET /api/care-events/3 — mostrar status COMPLETED com completedDate preenchida

FALA: "Aqui completo o evento de vacinação. O status muda para COMPLETED
e a data de conclusão é preenchida automaticamente. Demonstrei GET, POST,
PUT, PATCH e DELETE ao longo da apresentação — o CRUD completo funcionando
em nuvem com persistência no Oracle containerizado."

10. PUT /api/responsávels/3 — atualizar o responsável
11. DELETE /api/responsávels/1 — deletar (ou mostrar o endpoint)

FALA: "Aqui o PUT para atualizar um responsável e o DELETE para remoção.
Todas as operações estão refletidas no banco Oracle em nuvem."

### [CENA 6] Verificar containers e volume na VM via SSH

AÇÃO:
- ssh cloudadmin@[IP_PUBLICO]
- docker compose ps  →  mostrar ambos containers "running"
- docker volume ls   →  mostrar cloudcomputing-oracle-data
- docker ps          →  mostrar restart policy e tempo de uptime

FALA:
"Conecto via SSH na VM para confirmar os requisitos técnicos.
Os dois containers estão rodando em background com restart unless-stopped —
se a VM reiniciar, sobem automaticamente.
O volume nomeado 'cloudcomputing-oracle-data' está montado, garantindo a
persistência dos dados do Oracle mesmo que o container seja recriado.
E podemos ver que a aplicação roda com o usuário 1654, sem root."

## ─────────────────────────────────────────────
## PARTE 3 — DESTRUIÇÃO DA VM (Obrigatório) ~1 min
## ─────────────────────────────────────────────

### [CENA 7] Deletar o Resource Group

AÇÃO:
- No terminal local (ou Azure Portal):
  az group delete --name rg-cloudcomputing --yes
- Aguardar confirmação ou mostrar no Portal que a deleção iniciou

FALA:
"Por último, e de forma obrigatória conforme o enunciado, deleto a Máquina Virtual
e todos os recursos criados. Executo o comando az group delete, que remove
o Resource Group inteiro — VM, NSG, IP público e todos os recursos associados.
Manterei o print desta tela como evidência para o PDF de entrega."

AÇÃO:
- Tirar screenshot da tela mostrando o comando + confirmação de deleção
- (opcional) Abrir o Azure Portal e mostrar que o Resource Group sumiu

FALA:
"Aqui a evidência da remoção. Encerrando assim a demonstração do 1º Sprint.
Obrigado!"

## ─────────────────────────────────────────────
## CHECKLIST ANTES DE PUBLICAR NO YOUTUBE
## ─────────────────────────────────────────────
# [ ] Resolução mínima 720p (1280x720) — abaixo disso = ZERO
# [ ] Narração por voz em TODAS as cenas — sem narração = ZERO
# [ ] Mostrou o script CLI rodando (Cena 2)
# [ ] Mostrou docker compose ps com containers UP (Cena 6)
# [ ] Mostrou volume nomeado (Cena 6)
# [ ] Mostrou USER != root (Cena 6 ou Cena 3)
# [ ] Executou todas as operações do banco (GET, POST, PUT, DELETE, PATCH)
# [ ] Mostrou dados seedados no banco (Cena 5)
# [ ] Deletou a VM ao final (Cena 7)
# [ ] Screenshot da VM deletada salvo para o PDF
# [ ] Vídeo publicado como Não Listado ou Público no YouTube
# [ ] Link do vídeo inserido no PDF de entrega
