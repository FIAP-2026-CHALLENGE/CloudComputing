# 🎬 ROTEIRO COMPLETO DO VÍDEO — 1º Sprint
# DevOps Tools & Cloud Computing — FIAP
# ─────────────────────────────────────────────────────────────
# REGRAS OBRIGATÓRIAS ANTES DE COMEÇAR:
#   ✓ Resolução MÍNIMA 720p (1280×720) — abaixo disso = ZERO de nota
#   ✓ Narração por VOZ em TODAS as partes — sem voz = ZERO de nota
#   ✓ Mostre o terminal e o navegador claramente — sem zoom excessivo
#   ✓ NÃO grave em localhost — a aplicação deve rodar na VM Azure
# ─────────────────────────────────────────────────────────────

---

## ✅ PREPARAÇÃO ANTES DE GRAVAR (não aparece no vídeo)

### O que você precisa ter instalado na sua máquina:
- **Azure CLI** → https://docs.microsoft.com/cli/azure/install-azure-cli
- **Git**
- **Terminal** (PowerShell, Git Bash ou Terminal do macOS/Linux)
- **Navegador** (Chrome ou Edge)

### Passo único que você faz ANTES de ligar a gravação:

1. Abra o arquivo `azure-deploy.sh` e edite a linha:
   ```
   GITHUB_REPO="https://github.com/SEU_USUARIO/SEU_REPOSITORIO.git"
   ```
   Troque pelo link real do seu repositório GitHub (o mesmo que vai no PDF).

2. Salve o arquivo, faça commit e push para o GitHub:
   ```bash
   git add .
   git commit -m "feat: projeto finalizado"
   git push
   ```

> Isso é tudo. O script cuida do resto — ele faz login na Azure,
> cria a VM, instala o Docker, clona seu repositório e sobe tudo sozinho.
> Você NÃO precisa colocar senha, subscription ID nem nada no script.

---

## 🎬 PARTE 1 — SCRIPT AZURE CLI (Tarefa 01) | ~4 minutos

---

### 📍 CENA 1 — Mostrar o arquivo azure-deploy.sh
**Tela:** Editor de código (VS Code) com o arquivo `azure-deploy.sh` aberto

**FALA:**
> "Olá, sou [SEU NOME], RM [SEU RM]. Vou apresentar a entrega do
> primeiro sprint da disciplina DevOps Tools e Cloud Computing.
> Começando pela Tarefa 1: o script Azure CLI que provisiona toda
> a infraestrutura na nuvem automaticamente."

**AÇÃO:** Role devagar o arquivo de cima para baixo enquanto fala:

> "O script está dividido em 7 etapas sequenciais. No topo ficam
> as variáveis — o nome do Resource Group, a região eastus, o nome
> da VM, o tamanho Standard B2s que é suficiente para este projeto,
> e a URL do repositório GitHub que vai ser clonado automaticamente
> dentro da VM."

**AÇÃO:** Mostre a seção de variáveis (`RESOURCE_GROUP`, `VM_NAME`, `GITHUB_REPO`)

> "A etapa 1 faz o login na Azure via `az login`. A etapa 2 cria o
> Resource Group. A etapa 3 provisiona a VM Linux Ubuntu 24.04 com
> IP público e NSG. A etapa 4 abre as três portas necessárias: a 22
> para SSH, a 8081 para a API, e a 1521 para o Oracle."

**AÇÃO:** Role até a seção de NSG rules

> "A etapa 5 executa um script remoto dentro da VM usando o
> `run-command` do Azure CLI. Esse comando instala o Docker Engine,
> o docker-compose-plugin, o Git e o nano — sem precisar entrar na VM
> via SSH. A etapa 6 clona o repositório e sobe os containers com
> `docker compose up -d`. Por último, a etapa 7 imprime o resumo
> com o IP público e a URL do Swagger."

---

### 📍 CENA 2 — Executar o script ao vivo
**Tela:** Terminal aberto na pasta raiz do projeto

**AÇÃO:** Digite e execute:
```bash
chmod +x azure-deploy.sh
./azure-deploy.sh
```

**FALA:**
> "Vou executar o script agora. A primeira coisa que ele faz é o
> `az login`, que vai abrir o navegador para eu autenticar com
> minha conta Azure da FIAP."

**AÇÃO:** O navegador abre automaticamente → faça login com sua conta → volte para o terminal

**FALA:**
> "Login feito. O script agora segue automaticamente as etapas.
> Aqui o Resource Group sendo criado..."

**AÇÃO:** Aguarde e aponte para o terminal conforme cada etapa aparece:

- Quando aparecer `[2/7] Criando Resource Group`:
  > "Etapa 2 — Resource Group `rg-cloudcomputing` criado na região East US."

- Quando aparecer `[3/7] Criando VM`:
  > "Etapa 3 — Provisionando a VM Linux Ubuntu 24.04. Isso leva
  > cerca de um minuto..."

- Quando aparecer `VM criada com sucesso! IP Público: X.X.X.X`:
  > "VM criada. O IP público já aparece aqui no terminal."

- Quando aparecer `[4/7] Abrindo portas`:
  > "Etapa 4 — Criando as regras no NSG para liberar as portas
  > SSH, API e Oracle."

- Quando aparecer `[5/7] Instalando Docker`:
  > "Etapa 5 — O Azure CLI está executando comandos remotamente
  > dentro da VM para instalar o Docker e as ferramentas. Isso
  > leva alguns minutos."

- Quando aparecer `[6/7] Clonando repositório`:
  > "Etapa 6 — Clonando o repositório do GitHub dentro da VM
  > e subindo os containers com docker compose em background."

- Quando aparecer o resumo `[7/7] PROVISIONAMENTO CONCLUIDO`:
  > "Provisionamento concluído! Aqui está o resumo: o IP público
  > da VM, a URL da API com o Swagger, e o comando SSH para acessar
  > a máquina. O script da Tarefa 1 está completo."

**AÇÃO:** Deixe o terminal visível com o resumo final — tire um print desta tela.

---

## 🎬 PARTE 2 — APLICAÇÃO DOCKER EM NUVEM (Tarefa 02) | ~5 minutos

---

### 📍 CENA 3 — Mostrar Dockerfile e docker-compose.yml
**Tela:** VS Code com os arquivos abertos

**FALA:**
> "Agora a Tarefa 2: a aplicação rodando com Docker em nuvem.
> Vou mostrar primeiro a configuração dos containers."

**AÇÃO:** Abra o `Dockerfile`

> "O Dockerfile usa multi-stage build: a primeira etapa compila
> o projeto com a imagem SDK do .NET 10, e a segunda etapa usa
> só a imagem runtime — muito menor — para executar. Aqui embaixo
> tem uma linha importante: `USER 1654`. Isso garante que a aplicação
> roda com um usuário sem privilégios administrativos dentro do
> container, não como root."

**AÇÃO:** Destaque a linha `USER 1654`

**AÇÃO:** Abra o `docker-compose.yml`

> "No docker-compose temos dois serviços. O primeiro é o banco
> Oracle XE usando a imagem `gvenzl/oracle-xe`, que é a imagem
> sugerida pela disciplina. Ele tem um volume nomeado chamado
> `cloudcomputing-oracle-data` montado em `/opt/oracle/oradata` —
> isso é o que garante que os dados persistem mesmo se o container
> for recriado."

**AÇÃO:** Destaque o bloco `volumes`

> "O segundo serviço é a API .NET. Repare no `depends_on` com
> `condition: service_healthy` — a API só sobe depois que o Oracle
> passa no healthcheck, evitando erro de conexão na inicialização.
> Ambos os serviços têm `restart: unless-stopped`, então se a VM
> reiniciar eles sobem automaticamente."

**AÇÃO:** Destaque `depends_on`, `healthcheck` e `restart: unless-stopped`

> "Os dois containers estão na mesma rede bridge `cloudcomputing-network`,
> então a API se comunica com o Oracle pelo nome do serviço, não
> por IP."

---

### 📍 CENA 4 — Acessar o Swagger na nuvem
**Tela:** Navegador

**AÇÃO:** Abra o navegador e acesse:
```
http://[IP_PUBLICO_DA_VM]:8081/swagger
```
(use o IP que apareceu no terminal ao final da Cena 2)

**FALA:**
> "Aqui está a aplicação rodando em nuvem, acessível externamente
> pelo IP público da VM na porta 8081. O Swagger mostra as três
> entidades do sistema: Responsaveis, Animais e CareEvents."

**AÇÃO:** Mostre as três seções expandidas no Swagger

> "Este é um sistema de gestão de saúde animal. Responsaveis são
> os donos dos animais, Animais são os pets cadastrados com seus
> dados clínicos, e CareEvents registram eventos veterinários como
> vacinas, consultas e exames."

---

### 📍 CENA 5 — Demonstrar CRUD e banco de dados
**Tela:** Swagger no navegador

**FALA:**
> "Vou demonstrar o CRUD completo e mostrar cada operação
> refletida no banco Oracle."

#### ▶ GET — mostrar dados seedados

**AÇÃO:** Clique em `GET /api/responsaveis` → Execute

**FALA:**
> "O GET em responsaveis retorna os dois registros que foram
> inseridos automaticamente quando a aplicação subiu. As migrations
> do EF Core populam o banco com dados iniciais — Ana Paula Souza
> e Carlos Eduardo Lima já estão cadastrados."

**AÇÃO:** Clique em `GET /api/animais` → Execute

**FALA:**
> "Os dois animais também já estão no banco: Bolinha, um Labrador
> vinculado à Ana Paula, e Mia, uma Siamese vinculada ao Carlos."

**AÇÃO:** Clique em `GET /api/care-events` → Execute

**FALA:**
> "E os dois eventos de cuidado: uma vacina V10 já concluída para
> o Bolinha, e um check-up pendente para a Mia. Esses são os inserts
> com conteúdo significativo exigidos pela disciplina."

---

#### ▶ POST — criar novo registro

**AÇÃO:** Clique em `POST /api/responsaveis` → Try it out → preencha:
```json
{
  "name": "Fernanda Costa",
  "email": "fernanda.costa@email.com",
  "phone": "(11) 95555-1234",
  "cpf": "111.222.333-44"
}
```
→ Execute

**FALA:**
> "POST para criar um novo responsável. Retorno 201 com o objeto
> criado e o ID gerado pelo banco Oracle."

**AÇÃO:** Clique em `POST /api/animais` → Try it out → preencha:
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
→ Execute

**FALA:**
> "Cadastro o animal Rex vinculado ao responsável que acabei de criar.
> O campo `responsavelId` 3 é o ID que o banco atribuiu à Fernanda."

**AÇÃO:** Clique em `POST /api/care-events` → Try it out → preencha:
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
→ Execute

**FALA:**
> "Registro um evento de vacinação para o Rex. Status PENDING,
> prioridade HIGH."

---

#### ▶ GET — confirmar que o dado persiste

**AÇÃO:** `GET /api/care-events` → Execute

**FALA:**
> "O GET agora retorna três eventos — os dois do seed mais o que
> acabei de criar. Os dados estão persistidos no Oracle."

---

#### ▶ PUT — atualizar

**AÇÃO:** `PUT /api/responsaveis/3` → Try it out → preencha:
```json
{
  "name": "Fernanda Costa Silva",
  "email": "fernanda.silva@email.com",
  "phone": "(11) 95555-9999",
  "cpf": "111.222.333-44",
  "isActive": true
}
```
→ Execute

**FALA:**
> "PUT para atualizar o responsável. Retorno 204 No Content —
> a atualização foi feita com sucesso."

---

#### ▶ PATCH — operação especial

**AÇÃO:** `PATCH /api/care-events/3/complete` → Try it out → Execute

**FALA:**
> "O CareEvents tem um endpoint PATCH especial para marcar um
> evento como concluído. Ele preenche automaticamente a data de
> conclusão."

**AÇÃO:** `GET /api/care-events/3` → Execute

**FALA:**
> "Confirmando: o status mudou para COMPLETED e o campo
> completedDate foi preenchido automaticamente pelo sistema."

---

#### ▶ DELETE — remover

**AÇÃO:** `DELETE /api/care-events/3` → Try it out → Execute

**FALA:**
> "E o DELETE removendo o evento. Retorno 204. Demonstrei todas
> as operações: GET, POST, PUT, PATCH e DELETE — o CRUD completo
> funcionando em nuvem com persistência real no Oracle containerizado."

---

### 📍 CENA 6 — Verificar containers rodando via SSH
**Tela:** Terminal

**AÇÃO:** Conecte na VM via SSH:
```bash
ssh cloudadmin@[IP_PUBLICO_DA_VM]
```

**FALA:**
> "Vou conectar na VM via SSH para confirmar os requisitos técnicos
> diretamente no servidor."

**AÇÃO:** Execute:
```bash
docker compose ps
```

**FALA:**
> "Os dois containers estão com status `running` e com o uptime
> mostrando que estão rodando desde que o script os subiu."

**AÇÃO:** Execute:
```bash
docker volume ls
```

**FALA:**
> "O volume nomeado `cloudcomputing-oracle-data` está listado aqui,
> garantindo que os dados do Oracle persistem independente do
> ciclo de vida do container."

**AÇÃO:** Execute:
```bash
docker inspect cloudcomputing-api | grep -i user
```

**FALA:**
> "E confirmando que a API roda com o usuário 1654, não como root.
> Esse é um requisito explícito da disciplina."

---

## 🎬 PARTE 3 — DESTRUIÇÃO DA VM (Obrigatório) | ~2 minutos

---

### 📍 CENA 7 — Executar o azure-destroy.sh
**Tela:** Terminal local (não dentro da VM — saia da SSH primeiro com `exit`)

**FALA:**
> "Por último, e de forma obrigatória conforme o enunciado,
> vou deletar a Máquina Virtual e todos os recursos criados
> na Azure. Para isso tenho o script `azure-destroy.sh`."

**AÇÃO:** Execute:
```bash
./azure-destroy.sh
```

**FALA:**
> "O script primeiro verifica se estou logado e lista todos os
> recursos que serão removidos: a VM, o NSG, o IP público e os
> discos associados."

**AÇÃO:** Aguarde a listagem aparecer. Mostre a tabela de recursos na tela.

> "Aqui estão todos os recursos do Resource Group
> `rg-cloudcomputing`. Vou confirmar a exclusão digitando
> a palavra DELETAR."

**AÇÃO:** Digite `DELETAR` quando solicitado e pressione Enter.

> "Solicitação enviada. O script aguarda automaticamente a
> confirmação da Azure, verificando o status a cada 30 segundos."

**AÇÃO:** Aguarde o loop mostrar `Resource Group existe: false`

> "Confirmado — o Resource Group não existe mais. Todos os recursos
> foram removidos. Vou tirar o print desta tela agora para incluir
> no PDF de entrega como evidência obrigatória."

**AÇÃO:** 📸 **TIRE O PRINT DESTA TELA** — é a evidência para o PDF.

**FALA:**
> "Encerrando assim a demonstração do primeiro sprint da disciplina
> DevOps Tools e Cloud Computing. Obrigado."

---

## 📋 CHECKLIST FINAL — confira antes de publicar no YouTube

```
TÉCNICO:
[ ] Vídeo em resolução mínima 720p (1280×720)
[ ] Narração por voz em TODAS as cenas — sem exceção
[ ] Mostrou o azure-deploy.sh rodando do início ao fim
[ ] Mostrou o IP público da VM ao final do script
[ ] Mostrou Dockerfile com USER 1654
[ ] Mostrou docker-compose.yml com volume nomeado e healthcheck
[ ] Acessou o Swagger pelo IP público (não localhost)
[ ] Executou GET mostrando dados seedados no banco
[ ] Executou POST, PUT, PATCH e DELETE — todas operações visíveis
[ ] Conectou via SSH e mostrou docker compose ps (containers UP)
[ ] Mostrou docker volume ls com o volume nomeado
[ ] Executou azure-destroy.sh ao final
[ ] Aguardou e mostrou confirmação "Resource Group existe: false"
[ ] Tirou print da tela de confirmação de exclusão

PDF:
[ ] Print da exclusão da VM salvo
[ ] Link do repositório GitHub no PDF
[ ] Link do vídeo no YouTube no PDF
[ ] Diagrama de arquitetura com legendas no PDF
[ ] Folha de rosto com nome, RM e índice no PDF
```
