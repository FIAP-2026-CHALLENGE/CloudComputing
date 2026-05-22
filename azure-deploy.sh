#!/bin/bash
# =============================================================================
# azure-deploy.sh
# Script de provisionamento completo da infraestrutura Azure para o projeto
# CloudComputing (DotNet.Api + Oracle DB via Docker Compose)
# =============================================================================

set -e  # Encerra o script imediatamente se qualquer comando falhar

# =============================================================================
# VARIÁVEIS — ajuste conforme necessário
# =============================================================================
RESOURCE_GROUP="rg-cloudcomputing"
LOCATION="eastus"
VM_NAME="vm-cloudcomputing"
VM_IMAGE="Ubuntu2404"
VM_SIZE="Standard_B2s"
ADMIN_USER="cloudadmin"
VM_PUBLIC_IP_NAME="pip-cloudcomputing"
NSG_NAME="nsg-cloudcomputing"

# Porta da API (mapeada no docker-compose.yml: host 8081 → container 8080)
API_PORT="8081"
# Porta SSH padrão
SSH_PORT="22"
# Porta Oracle (caso precise acessar externamente)
ORACLE_PORT="1521"

# Repositório GitHub do projeto
GITHUB_REPO="https://github.com/FIAP-2026-CHALLENGE/CloudComputing"

# =============================================================================
# 1) LOGIN E SELEÇÃO DE SUBSCRIPTION
# =============================================================================
echo ""
echo "======================================================================"
echo " [1/7] Autenticando na Azure..."
echo "======================================================================"

az login --only-show-errors
az account show --output table

# =============================================================================
# 2) CRIAR RESOURCE GROUP
# =============================================================================
echo ""
echo "======================================================================"
echo " [2/7] Criando Resource Group: $RESOURCE_GROUP em $LOCATION..."
echo "======================================================================"

az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --output table

# =============================================================================
# 3) CRIAR VM LINUX (Ubuntu 24.04)
# =============================================================================
echo ""
echo "======================================================================"
echo " [3/7] Criando VM Linux ($VM_NAME)..."
echo "======================================================================"

az vm create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$VM_NAME" \
  --image "$VM_IMAGE" \
  --size "$VM_SIZE" \
  --admin-username "$ADMIN_USER" \
  --generate-ssh-keys \
  --public-ip-address "$VM_PUBLIC_IP_NAME" \
  --public-ip-sku Standard \
  --nsg "$NSG_NAME" \
  --output table

# Obtém o IP público da VM
VM_PUBLIC_IP=$(az vm show \
  --resource-group "$RESOURCE_GROUP" \
  --name "$VM_NAME" \
  --show-details \
  --query publicIps \
  --output tsv)

echo ""
echo ">>> VM criada com sucesso! IP Público: $VM_PUBLIC_IP"

# =============================================================================
# 4) ABRIR PORTAS NO NSG
# =============================================================================
echo ""
echo "======================================================================"
echo " [4/7] Abrindo portas necessárias no NSG ($NSG_NAME)..."
echo "======================================================================"

# Porta SSH (22) — geralmente já aberta por padrão, criamos explicitamente
az network nsg rule create \
  --resource-group "$RESOURCE_GROUP" \
  --nsg-name "$NSG_NAME" \
  --name "Allow-SSH" \
  --protocol tcp \
  --priority 1000 \
  --destination-port-range "$SSH_PORT" \
  --access Allow \
  --direction Inbound \
  --output table

# Porta da API (8081)
az network nsg rule create \
  --resource-group "$RESOURCE_GROUP" \
  --nsg-name "$NSG_NAME" \
  --name "Allow-API" \
  --protocol tcp \
  --priority 1010 \
  --destination-port-range "$API_PORT" \
  --access Allow \
  --direction Inbound \
  --output table

# Porta Oracle (1521) — opcional, útil para testes externos
az network nsg rule create \
  --resource-group "$RESOURCE_GROUP" \
  --nsg-name "$NSG_NAME" \
  --name "Allow-Oracle" \
  --protocol tcp \
  --priority 1020 \
  --destination-port-range "$ORACLE_PORT" \
  --access Allow \
  --direction Inbound \
  --output table

echo ""
echo ">>> Portas SSH ($SSH_PORT), API ($API_PORT) e Oracle ($ORACLE_PORT) abertas."

# =============================================================================
# 5) INSTALAR DOCKER E FERRAMENTAS NA VM VIA cloud-init / run-command
# =============================================================================
echo ""
echo "======================================================================"
echo " [5/7] Instalando Docker, Git, nano e docker-compose-plugin na VM..."
echo "======================================================================"

az vm run-command invoke \
  --resource-group "$RESOURCE_GROUP" \
  --name "$VM_NAME" \
  --command-id RunShellScript \
  --scripts '
    set -e

    echo ">>> Atualizando pacotes..."
    apt-get update -y

    echo ">>> Instalando dependencias do Docker..."
    apt-get install -y ca-certificates curl gnupg lsb-release git nano

    echo ">>> Adicionando repositorio oficial do Docker..."
    install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg \
      | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    chmod a+r /etc/apt/keyrings/docker.gpg

    echo \
      "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
      https://download.docker.com/linux/ubuntu \
      $(lsb_release -cs) stable" \
      | tee /etc/apt/sources.list.d/docker.list > /dev/null

    echo ">>> Instalando Docker Engine e docker-compose-plugin..."
    apt-get update -y
    apt-get install -y docker-ce docker-ce-cli containerd.io \
      docker-buildx-plugin docker-compose-plugin

    echo ">>> Habilitando e iniciando Docker..."
    systemctl enable docker
    systemctl start docker

    echo ">>> Adicionando usuario cloudadmin ao grupo docker..."
    usermod -aG docker cloudadmin

    echo ">>> Versoes instaladas:"
    docker --version
    docker compose version
    git --version
    nano --version | head -1
  ' \
  --output table

echo ""
echo ">>> Docker, Git e nano instalados com sucesso na VM."

# =============================================================================
# 6) CLONAR O REPOSITÓRIO E SUBIR A APLICAÇÃO COM DOCKER COMPOSE
# =============================================================================
echo ""
echo "======================================================================"
echo " [6/7] Clonando repositorio e iniciando aplicacao com Docker Compose..."
echo "======================================================================"

az vm run-command invoke \
  --resource-group "$RESOURCE_GROUP" \
  --name "$VM_NAME" \
  --command-id RunShellScript \
  --scripts "
    set -e

    echo '>>> Clonando repositorio...'
    cd /home/cloudadmin
    git clone $GITHUB_REPO app
    cd app

    echo '>>> Subindo containers em background (docker compose up -d)...'
    docker compose up -d --build

    echo '>>> Containers em execucao:'
    docker compose ps
  " \
  --output table

# =============================================================================
# 7) RESUMO FINAL
# =============================================================================
echo ""
echo "======================================================================"
echo " [7/7] PROVISIONAMENTO CONCLUIDO!"
echo "======================================================================"
echo ""
echo "  Resource Group : $RESOURCE_GROUP"
echo "  VM             : $VM_NAME"
echo "  IP Público     : $VM_PUBLIC_IP"
echo "  API URL        : http://$VM_PUBLIC_IP:$API_PORT/swagger"
echo "  SSH            : ssh $ADMIN_USER@$VM_PUBLIC_IP"
echo ""
echo "======================================================================"
echo " LEMBRETE: Ao finalizar a apresentacao, execute o script de limpeza:"
echo "   az group delete --name $RESOURCE_GROUP --yes --no-wait"
echo "======================================================================"
