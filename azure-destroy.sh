#!/bin/bash
set -e
# =============================================================================
# azure-destroy.sh
# Remove todos os recursos Azure criados pelo azure-deploy.sh, evitando
# custo residual na assinatura (importante em conta Azure for Students).
# =============================================================================

RM="562822"  # ALTERE PARA SEU RM (deve ser igual ao usado no azure-deploy.sh)
RESOURCE_GROUP="rg-cloudcomputing-${RM}"

echo "Isso vai apagar TODOS os recursos dentro de: $RESOURCE_GROUP"
read -p "Confirma? (digite 'sim' para prosseguir): " CONFIRMACAO

if [ "$CONFIRMACAO" != "sim" ]; then
  echo "Cancelado."
  exit 0
fi

az group delete --name "$RESOURCE_GROUP" --yes --no-wait

echo "Exclusão do Resource Group '$RESOURCE_GROUP' solicitada (rodando em background)."
echo "Use 'az group show --name $RESOURCE_GROUP' para acompanhar até ele sumir."
