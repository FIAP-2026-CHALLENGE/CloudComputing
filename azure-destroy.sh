#!/bin/bash
# =============================================================================
# azure-destroy.sh
# Script de remoção completa da infraestrutura Azure criada pelo azure-deploy.sh
# Obrigatório executar ao final da apresentação — conforme enunciado da disciplina
# =============================================================================

# =============================================================================
# VARIÁVEL — deve ser o mesmo Resource Group usado no azure-deploy.sh
# =============================================================================
RESOURCE_GROUP="rg-cloudcomputing"

# =============================================================================
# VERIFICAÇÃO: Azure CLI instalado e logado
# =============================================================================
echo ""
echo "======================================================================"
echo " azure-destroy.sh — Remoção da infraestrutura Azure"
echo "======================================================================"
echo ""

if ! command -v az &> /dev/null; then
  echo "[ERRO] Azure CLI não encontrado. Instale em: https://aka.ms/installazurecli"
  exit 1
fi

# Verifica se está logado
ACCOUNT=$(az account show --query name -o tsv 2>/dev/null)
if [ -z "$ACCOUNT" ]; then
  echo "[INFO] Nenhuma sessão ativa. Fazendo login..."
  az login --only-show-errors
fi

echo "[INFO] Conta Azure: $(az account show --query name -o tsv)"
echo "[INFO] Subscription: $(az account show --query id -o tsv)"
echo ""

# =============================================================================
# VERIFICAR SE O RESOURCE GROUP EXISTE
# =============================================================================
RG_EXISTS=$(az group exists --name "$RESOURCE_GROUP")

if [ "$RG_EXISTS" = "false" ]; then
  echo "[AVISO] Resource Group '$RESOURCE_GROUP' não encontrado."
  echo "        Pode já ter sido deletado ou o nome está diferente."
  echo ""
  echo "Resource Groups disponíveis na sua conta:"
  az group list --query "[].name" -o tsv
  exit 0
fi

# =============================================================================
# LISTAR RECURSOS QUE SERÃO DELETADOS (para evidência no vídeo/PDF)
# =============================================================================
echo "======================================================================"
echo " Recursos que serão REMOVIDOS dentro de '$RESOURCE_GROUP':"
echo "======================================================================"
az resource list \
  --resource-group "$RESOURCE_GROUP" \
  --query "[].{Nome:name, Tipo:type, Local:location}" \
  --output table
echo ""

# =============================================================================
# CONFIRMAÇÃO EXPLÍCITA (importante mostrar no vídeo)
# =============================================================================
echo "======================================================================"
echo " ATENÇÃO: Esta operação é IRREVERSÍVEL."
echo " Todos os recursos acima serão permanentemente deletados."
echo "======================================================================"
echo ""
read -p " Digite 'DELETAR' para confirmar a exclusão: " CONFIRMACAO

if [ "$CONFIRMACAO" != "DELETAR" ]; then
  echo ""
  echo "[CANCELADO] Nenhum recurso foi removido."
  exit 0
fi

# =============================================================================
# DELETAR O RESOURCE GROUP (remove VM, NSG, IP público e todos os recursos)
# =============================================================================
echo ""
echo "======================================================================"
echo " Iniciando exclusão do Resource Group '$RESOURCE_GROUP'..."
echo " (Este processo leva alguns minutos)"
echo "======================================================================"
echo ""

az group delete \
  --name "$RESOURCE_GROUP" \
  --yes \
  --no-wait

echo ""
echo "======================================================================"
echo " Solicitação de exclusão enviada com sucesso!"
echo "======================================================================"
echo ""
echo " A exclusão ocorre em segundo plano na Azure."
echo " Para confirmar que foi removido, execute:"
echo ""
echo "   az group exists --name $RESOURCE_GROUP"
echo ""
echo " Resultado esperado: false"
echo ""

# =============================================================================
# AGUARDAR CONFIRMAÇÃO E MOSTRAR STATUS FINAL (para evidência no vídeo)
# =============================================================================
echo " Aguardando confirmação da Azure (pode levar 1-3 minutos)..."
echo ""

for i in 1 2 3 4 5 6; do
  sleep 30
  STATUS=$(az group exists --name "$RESOURCE_GROUP")
  echo " [$(date '+%H:%M:%S')] Resource Group existe: $STATUS"
  if [ "$STATUS" = "false" ]; then
    break
  fi
done

echo ""
if [ "$(az group exists --name "$RESOURCE_GROUP")" = "false" ]; then
  echo "======================================================================"
  echo " CONFIRMADO: Resource Group '$RESOURCE_GROUP' foi removido com sucesso!"
  echo " Tire o print desta tela para o PDF de entrega."
  echo "======================================================================"
else
  echo "======================================================================"
  echo " A exclusão ainda está em andamento na Azure."
  echo " Verifique no Portal Azure em alguns minutos e tire o print."
  echo " Portal: https://portal.azure.com/#blade/HubsExtension/BrowseResourceGroups"
  echo "======================================================================"
fi

echo ""
