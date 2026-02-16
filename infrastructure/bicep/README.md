# Guide de déploiement - Infrastructure Azure

Ce guide explique comment déployer l'infrastructure Azure pour l'application Geocatis Pharmacy Management.

## Prérequis

- Azure CLI installé et configuré
- Compte Azure avec les permissions appropriées
- Bicep CLI installé
- PowerShell ou Bash

## Structure des fichiers

```
infrastructure/bicep/
├── main.bicep                      # Template principal
├── main.parameters.dev.json        # Paramètres pour dev
├── main.parameters.prod.json       # Paramètres pour prod
└── README.md                       # Ce fichier
```

## Déploiement

### 1. Se connecter à Azure

```bash
az login
az account set --subscription "<subscription-id>"
```

### 2. Créer le groupe de ressources

```bash
# Pour développement
az group create \
  --name geocatis-dev-rg \
  --location westeurope

# Pour production
az group create \
  --name geocatis-prod-rg \
  --location westeurope
```

### 3. Créer les secrets dans Key Vault (pré-déploiement)

Avant le premier déploiement, créez un Key Vault temporaire pour stocker les credentials SQL :

```bash
# Créer un Key Vault temporaire
az keyvault create \
  --name geocatis-deploy-kv \
  --resource-group geocatis-dev-rg \
  --location westeurope

# Créer les secrets SQL
az keyvault secret set \
  --vault-name geocatis-deploy-kv \
  --name SqlAdminLogin \
  --value "sqladmin"

az keyvault secret set \
  --vault-name geocatis-deploy-kv \
  --name SqlAdminPassword \
  --value "<mot-de-passe-fort>"
```

### 4. Mettre à jour les fichiers de paramètres

Modifier `main.parameters.dev.json` ou `main.parameters.prod.json` pour référencer votre Key Vault :

```json
{
  "sqlAdminLogin": {
    "reference": {
      "keyVault": {
        "id": "/subscriptions/{votre-subscription-id}/resourceGroups/geocatis-dev-rg/providers/Microsoft.KeyVault/vaults/geocatis-deploy-kv"
      },
      "secretName": "SqlAdminLogin"
    }
  }
}
```

### 5. Valider le template

```bash
# Valider pour dev
az deployment group validate \
  --resource-group geocatis-dev-rg \
  --template-file main.bicep \
  --parameters main.parameters.dev.json

# Valider pour prod
az deployment group validate \
  --resource-group geocatis-prod-rg \
  --template-file main.bicep \
  --parameters main.parameters.prod.json
```

### 6. Déployer l'infrastructure

```bash
# Déploiement dev
az deployment group create \
  --resource-group geocatis-dev-rg \
  --template-file main.bicep \
  --parameters main.parameters.dev.json \
  --name geocatis-dev-deployment

# Déploiement prod
az deployment group create \
  --resource-group geocatis-prod-rg \
  --template-file main.bicep \
  --parameters main.parameters.prod.json \
  --name geocatis-prod-deployment
```

### 7. Récupérer les outputs

```bash
az deployment group show \
  --resource-group geocatis-dev-rg \
  --name geocatis-dev-deployment \
  --query properties.outputs
```

## Déploiement simplifié (sans Key Vault pré-existant)

Si vous préférez fournir les paramètres directement :

```bash
az deployment group create \
  --resource-group geocatis-dev-rg \
  --template-file main.bicep \
  --parameters \
    projectName=geocatis \
    environment=dev \
    location=westeurope \
    appServiceSku=S1 \
    sqlDatabaseSku=S2 \
    sqlAdminLogin=sqladmin \
    sqlAdminPassword='VotreMotDePasseSecurise123!'
```

⚠️ **Attention** : Cette méthode expose les credentials dans l'historique de commandes. Utilisez la méthode Key Vault en production.

## Configuration post-déploiement

### 1. Configurer les connection strings dans les App Services

```bash
# Récupérer la connection string SQL
CONN_STRING=$(az sql db show-connection-string \
  --client ado.net \
  --server geocatis-dev-sql \
  --name geocatisDB \
  --output tsv)

# Configurer dans l'API
az webapp config connection-string set \
  --resource-group geocatis-dev-rg \
  --name geocatis-dev-api \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="$CONN_STRING"
```

### 2. Configurer les secrets dans Key Vault

```bash
# Storage Account connection string
STORAGE_CONN=$(az storage account show-connection-string \
  --resource-group geocatis-dev-rg \
  --name geocatisdevsa \
  --output tsv)

az keyvault secret set \
  --vault-name geocatis-dev-kv \
  --name StorageConnectionString \
  --value "$STORAGE_CONN"

# Service Bus connection string
SB_CONN=$(az servicebus namespace authorization-rule keys list \
  --resource-group geocatis-dev-rg \
  --namespace-name geocatis-dev-sb \
  --name RootManageSharedAccessKey \
  --query primaryConnectionString \
  --output tsv)

az keyvault secret set \
  --vault-name geocatis-dev-kv \
  --name ServiceBusConnectionString \
  --value "$SB_CONN"
```

### 3. Configurer les permissions Key Vault

Les Managed Identities des App Services et Functions ont déjà les permissions nécessaires grâce aux role assignments dans le template.

## Monitoring et logs

### Activer les logs de diagnostic

```bash
# Pour l'API App Service
az monitor diagnostic-settings create \
  --resource $(az webapp show --resource-group geocatis-dev-rg --name geocatis-dev-api --query id -o tsv) \
  --name apiLogs \
  --workspace $(az monitor log-analytics workspace show --resource-group geocatis-dev-rg --workspace-name geocatis-dev-logs --query id -o tsv) \
  --logs '[{"category": "AppServiceHTTPLogs", "enabled": true}, {"category": "AppServiceConsoleLogs", "enabled": true}]' \
  --metrics '[{"category": "AllMetrics", "enabled": true}]'
```

## Mise à jour de l'infrastructure

Pour mettre à jour l'infrastructure existante, modifiez le template Bicep et redéployez :

```bash
az deployment group create \
  --resource-group geocatis-dev-rg \
  --template-file main.bicep \
  --parameters main.parameters.dev.json \
  --mode Incremental
```

## Suppression de l'infrastructure

⚠️ **ATTENTION** : Cette action supprime toutes les ressources et données !

```bash
# Supprimer le groupe de ressources complet
az group delete \
  --name geocatis-dev-rg \
  --yes \
  --no-wait
```

## Coûts estimés

Consultez le fichier `/docs/ARCHITECTURE.md` pour les estimations de coûts par environnement.

## Troubleshooting

### Erreur : "The template deployment failed because of policy violation"

Vérifiez les policies Azure de votre organisation. Vous pourriez avoir besoin d'une exemption pour certaines ressources.

### Erreur : "The subscription is not registered to use namespace 'Microsoft.XXX'"

Enregistrez le provider nécessaire :

```bash
az provider register --namespace Microsoft.Web
az provider register --namespace Microsoft.Sql
az provider register --namespace Microsoft.Storage
az provider register --namespace Microsoft.KeyVault
az provider register --namespace Microsoft.ServiceBus
az provider register --namespace Microsoft.Insights
```

### Erreur de quota dépassé

Demandez une augmentation de quota via le portail Azure ou contactez le support.

## Support

Pour toute question ou problème :
- Consulter la documentation Azure : https://docs.microsoft.com/azure
- Ouvrir une issue dans le repository
- Contacter l'équipe DevOps

## Références

- [Azure Bicep Documentation](https://docs.microsoft.com/azure/azure-resource-manager/bicep/)
- [Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [Azure SQL Database](https://docs.microsoft.com/azure/azure-sql/)
- [Azure Key Vault](https://docs.microsoft.com/azure/key-vault/)
- [Azure Service Bus](https://docs.microsoft.com/azure/service-bus-messaging/)
