#!/bin/bash
set -e
# =============================================================================
# azure-deploy.sh
# Provisiona ACR + ACI (grupo de containers com API .NET + MySQL) na Azure
# para o projeto CloudComputing (Sprint 3 - Cloud & DevOps).
# =============================================================================

# =============================================================================
# VARIÁVEIS — ajuste RM e LOCATION conforme sua conta Azure for Students
# =============================================================================
RM="562822"                                   # ALTERE PARA SEU RM
RESOURCE_GROUP="rg-cloudcomputing-${RM}"
LOCATION="eastus2"                            # confirmado via política sys.regionrestriction desta assinatura

ACR_NAME="acrcloudcomputing${RM}"             # só letras/números, sem hífen
CONTAINER_GROUP="cloudcomputing-aci-${RM}"
DNS_LABEL="cloudcomputing-${RM}"              # vira parte da URL pública
IMAGE_TAG="v1"

STORAGE_ACCOUNT="stcloudcomputing${RM}"       # só letras minúsculas/números, até 24 chars
FILE_SHARE="cloudcomputing-mysql-data"

# Carrega as credenciais do banco do seu .env (mesmo arquivo usado no Docker local)
if [ ! -f .env ]; then
  echo "Arquivo .env não encontrado na raiz do projeto. Copie o .env.example e preencha antes de continuar."
  exit 1
fi
set -a
source .env
set +a

echo "==> Verificando login na Azure..."
az account show > /dev/null || { echo "Rode 'az login' antes de continuar."; exit 1; }

# =============================================================================
# 1) RESOURCE GROUP
# =============================================================================
echo "==> Criando Resource Group..."
az group create --name "$RESOURCE_GROUP" --location "$LOCATION" --output table

# =============================================================================
# 2) ACR (Azure Container Registry)
# =============================================================================
echo "==> Registrando provider do ACR (idempotente, não falha se já estiver registrado)..."
az provider register --namespace Microsoft.ContainerRegistry

echo "==> Criando ACR..."
az acr create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$ACR_NAME" \
  --sku Basic \
  --location "$LOCATION" \
  --admin-enabled true \
  --output table

ACR_SERVER=$(az acr show --name "$ACR_NAME" --resource-group "$RESOURCE_GROUP" --query loginServer --output tsv)
ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --resource-group "$RESOURCE_GROUP" --query username --output tsv)
ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --resource-group "$RESOURCE_GROUP" --query "passwords[0].value" --output tsv)

echo "ACR Server: $ACR_SERVER"

# =============================================================================
# 3) BUILD + PUSH da imagem da API
# =============================================================================
echo "==> Fazendo login Docker no ACR..."
ACR_TOKEN=$(az acr login --name "$ACR_NAME" --expose-token --query accessToken -o tsv)
echo "$ACR_TOKEN" | docker login "$ACR_SERVER" --username 00000000-0000-0000-0000-000000000000 --password-stdin

echo "==> Buildando a imagem da API..."
docker build -t cloudcomputing-api:"$IMAGE_TAG" -f Dockerfile .

echo "==> Taggeando e enviando para o ACR..."
docker tag cloudcomputing-api:"$IMAGE_TAG" "$ACR_SERVER/cloudcomputing-api:$IMAGE_TAG"
docker push "$ACR_SERVER/cloudcomputing-api:$IMAGE_TAG"

# =============================================================================
# 4) STORAGE ACCOUNT + FILE SHARE (persistência do MySQL em nuvem)
# =============================================================================
echo "==> Criando Storage Account..."
az storage account create \
  --name "$STORAGE_ACCOUNT" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --sku Standard_LRS \
  --output table

STORAGE_KEY=$(az storage account keys list \
  --resource-group "$RESOURCE_GROUP" \
  --account-name "$STORAGE_ACCOUNT" \
  --query "[0].value" --output tsv)

echo "==> Criando File Share para os dados do MySQL..."
az storage share create \
  --name "$FILE_SHARE" \
  --account-name "$STORAGE_ACCOUNT" \
  --account-key "$STORAGE_KEY"

# =============================================================================
# 5) GERAR O aci-deploy.yaml FINAL (substitui as variáveis no template)
# =============================================================================
echo "==> Gerando aci-deploy.yaml a partir do template..."
export LOCATION CONTAINER_GROUP DNS_LABEL ACR_SERVER ACR_USERNAME ACR_PASSWORD IMAGE_TAG
export FILE_SHARE STORAGE_ACCOUNT STORAGE_KEY
export MYSQL_ROOT_PASSWORD MYSQL_DATABASE MYSQL_USER MYSQL_PASSWORD

envsubst < aci-deploy.yaml.template > aci-deploy.yaml

# =============================================================================
# 6) CRIAR O GRUPO DE CONTAINERS (ACI)
# =============================================================================
echo "==> Criando o grupo de containers (API + MySQL) via ACI..."
az container create --resource-group "$RESOURCE_GROUP" --file aci-deploy.yaml

# =============================================================================
# 7) RESULTADO
# =============================================================================
FQDN=$(az container show --resource-group "$RESOURCE_GROUP" --name "$CONTAINER_GROUP" --query ipAddress.fqdn --output tsv)
echo ""
echo "===================================================================="
echo "Deploy concluído."
echo "Swagger:      http://$FQDN:8080/swagger"
echo "Health check: http://$FQDN:8080/health"
echo "===================================================================="