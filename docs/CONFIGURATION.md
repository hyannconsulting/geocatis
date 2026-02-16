# Configuration Guide - Geocatis Pharmacy Management

Ce guide explique comment configurer l'application Geocatis pour différents environnements.

## Table des matières

1. [Configuration de développement](#configuration-de-développement)
2. [Configuration de production](#configuration-de-production)
3. [Variables d'environnement](#variables-denvironnement)
4. [Secrets et sécurité](#secrets-et-sécurité)
5. [Configuration Azure](#configuration-azure)

---

## Configuration de développement

### appsettings.Development.json (API)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GeocatisDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "https://localhost:5001",
      "http://localhost:5000"
    ]
  },
  "Azure": {
    "KeyVault": {
      "Enabled": false
    },
    "ApplicationInsights": {
      "Enabled": false
    },
    "BlobStorage": {
      "ConnectionString": "UseDevelopmentStorage=true",
      "ContainerNames": {
        "Documents": "documents",
        "Images": "images",
        "Reports": "reports"
      }
    }
  },
  "Authentication": {
    "UseAzureADB2C": false,
    "Jwt": {
      "Issuer": "https://localhost:7001",
      "Audience": "geocatis-api",
      "SecretKey": "your-development-secret-key-min-32-chars"
    }
  },
  "Pharmacy": {
    "DefaultCurrency": "XOF",
    "DefaultLanguage": "fr-CI",
    "TaxRate": 18.0,
    "DateFormat": "dd/MM/yyyy",
    "TimeZone": "GMT Standard Time"
  }
}
```

### appsettings.Development.json (Web)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Api": {
    "BaseUrl": "https://localhost:7001",
    "Timeout": 30
  },
  "Azure": {
    "ApplicationInsights": {
      "Enabled": false
    }
  }
}
```

---

## Configuration de production

### appsettings.Production.json (API)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/SqlConnectionString/)"
  },
  "AllowedHosts": "*.azurewebsites.net,*.geocatis.com",
  "Cors": {
    "AllowedOrigins": [
      "https://geocatis-prod-web.azurewebsites.net",
      "https://www.geocatis.com"
    ]
  },
  "Azure": {
    "KeyVault": {
      "Enabled": true,
      "VaultUri": "https://geocatis-prod-kv.vault.azure.net/"
    },
    "ApplicationInsights": {
      "Enabled": true,
      "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/AppInsightsConnectionString/)"
    },
    "BlobStorage": {
      "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/StorageConnectionString/)",
      "ContainerNames": {
        "Documents": "documents",
        "Images": "images",
        "Reports": "reports"
      }
    },
    "ServiceBus": {
      "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/ServiceBusConnectionString/)",
      "Queues": {
        "Notifications": "notifications",
        "Reports": "reports"
      },
      "Topics": {
        "InventoryEvents": "inventory-events"
      }
    }
  },
  "Authentication": {
    "UseAzureADB2C": true,
    "AzureAdB2C": {
      "Instance": "https://geocatisb2c.b2clogin.com/",
      "Domain": "geocatisb2c.onmicrosoft.com",
      "TenantId": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/ADB2CTenantId/)",
      "ClientId": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/ADB2CClientId/)",
      "SignUpSignInPolicyId": "B2C_1_SignUpSignIn"
    }
  },
  "Pharmacy": {
    "DefaultCurrency": "XOF",
    "DefaultLanguage": "fr-CI",
    "TaxRate": 18.0,
    "DateFormat": "dd/MM/yyyy",
    "TimeZone": "GMT Standard Time"
  },
  "Integrations": {
    "OrangeMoney": {
      "Enabled": true,
      "ApiUrl": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/OrangeMoneyApiUrl/)",
      "ApiKey": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/OrangeMoneyApiKey/)",
      "MerchantId": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/OrangeMoneyMerchantId/)"
    },
    "MTNMobileMoney": {
      "Enabled": true,
      "ApiUrl": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/MTNMoneyApiUrl/)",
      "ApiKey": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/MTNMoneyApiKey/)"
    },
    "AfricasTalking": {
      "Enabled": true,
      "ApiKey": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/AfricasTalkingApiKey/)",
      "Username": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/AfricasTalkingUsername/)"
    },
    "SendGrid": {
      "Enabled": true,
      "ApiKey": "@Microsoft.KeyVault(SecretUri=https://geocatis-prod-kv.vault.azure.net/secrets/SendGridApiKey/)",
      "FromEmail": "noreply@geocatis.com",
      "FromName": "Geocatis Pharmacy"
    }
  },
  "HealthChecks": {
    "Enabled": true,
    "Path": "/health"
  },
  "RateLimiting": {
    "Enabled": true,
    "PermitLimit": 100,
    "Window": 60,
    "QueueLimit": 10
  }
}
```

---

## Variables d'environnement

### Variables système requises

```bash
# Azure
AZURE_TENANT_ID=your-tenant-id
AZURE_CLIENT_ID=your-client-id
AZURE_CLIENT_SECRET=your-client-secret

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443;http://+:80

# Base de données
DB_SERVER=geocatis-prod-sql.database.windows.net
DB_NAME=GeocatisDB
DB_USER=sqladmin
DB_PASSWORD=your-secure-password

# Monitoring
APPLICATIONINSIGHTS_CONNECTION_STRING=your-connection-string
```

### Variables dans Azure App Service

Configuration via Azure Portal ou CLI :

```bash
# Définir les paramètres d'application
az webapp config appsettings set \
  --resource-group geocatis-prod-rg \
  --name geocatis-prod-api \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    WEBSITE_TIME_ZONE="GMT Standard Time" \
    WEBSITE_LOAD_CERTIFICATES=* \
    WEBSITE_RUN_FROM_PACKAGE=1
```

---

## Secrets et sécurité

### Gestion des secrets en développement

Utiliser User Secrets pour éviter de committer des secrets :

```bash
# Initialiser User Secrets
cd Geocatis.API
dotnet user-secrets init

# Ajouter des secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GeocatisDB;..."
dotnet user-secrets set "Authentication:Jwt:SecretKey" "your-secret-key"
dotnet user-secrets set "Azure:BlobStorage:ConnectionString" "DefaultEndpointsProtocol=https;..."

# Lister les secrets
dotnet user-secrets list

# Supprimer un secret
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

### Gestion des secrets en production (Azure Key Vault)

```bash
# Créer des secrets dans Key Vault
az keyvault secret set \
  --vault-name geocatis-prod-kv \
  --name SqlConnectionString \
  --value "Server=tcp:geocatis-prod-sql.database.windows.net,1433;Database=GeocatisDB;..."

az keyvault secret set \
  --vault-name geocatis-prod-kv \
  --name StorageConnectionString \
  --value "DefaultEndpointsProtocol=https;AccountName=geocatisprodsa;..."

az keyvault secret set \
  --vault-name geocatis-prod-kv \
  --name OrangeMoneyApiKey \
  --value "your-orange-money-api-key"
```

### Accès Key Vault depuis l'application

L'application utilise Managed Identity pour accéder à Key Vault sans credentials :

```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["Azure:KeyVault:VaultUri"]),
    new DefaultAzureCredential());
```

---

## Configuration Azure

### Azure SQL Database

```bash
# Configurer le firewall
az sql server firewall-rule create \
  --resource-group geocatis-prod-rg \
  --server geocatis-prod-sql \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Activer Advanced Data Security
az sql server ad-admin create \
  --resource-group geocatis-prod-rg \
  --server-name geocatis-prod-sql \
  --display-name "SQL Admin" \
  --object-id <azure-ad-user-object-id>
```

### Azure Blob Storage

```bash
# Configurer CORS pour le storage
az storage cors add \
  --services b \
  --methods GET POST PUT \
  --origins https://geocatis-prod-web.azurewebsites.net \
  --allowed-headers "*" \
  --exposed-headers "*" \
  --max-age 3600 \
  --account-name geocatisprodsa
```

### Application Insights

```bash
# Configurer les alertes
az monitor metrics alert create \
  --name HighErrorRate \
  --resource-group geocatis-prod-rg \
  --scopes /subscriptions/{sub-id}/resourceGroups/geocatis-prod-rg/providers/Microsoft.Web/sites/geocatis-prod-api \
  --condition "avg requests/failed > 5" \
  --window-size 5m \
  --evaluation-frequency 1m
```

### Service Bus

```bash
# Créer des queues additionnelles
az servicebus queue create \
  --resource-group geocatis-prod-rg \
  --namespace-name geocatis-prod-sb \
  --name orders \
  --max-delivery-count 10 \
  --default-message-time-to-live P7D
```

---

## Vérification de la configuration

### Script de vérification

```bash
#!/bin/bash
# verify-config.sh

echo "Vérification de la configuration Geocatis..."

# Vérifier les variables d'environnement
if [ -z "$ASPNETCORE_ENVIRONMENT" ]; then
    echo "❌ ASPNETCORE_ENVIRONMENT non définie"
else
    echo "✅ ASPNETCORE_ENVIRONMENT: $ASPNETCORE_ENVIRONMENT"
fi

# Vérifier la connectivité SQL
echo "Vérification de la base de données..."
sqlcmd -S $DB_SERVER -U $DB_USER -P $DB_PASSWORD -Q "SELECT 1" > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ Connexion SQL OK"
else
    echo "❌ Échec de connexion SQL"
fi

# Vérifier Key Vault
echo "Vérification de Key Vault..."
az keyvault secret list --vault-name geocatis-prod-kv > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ Key Vault accessible"
else
    echo "❌ Key Vault inaccessible"
fi

echo "Vérification terminée."
```

---

## Dépannage

### Problème : Connection string non trouvée

**Solution** : Vérifier que le secret existe dans Key Vault et que Managed Identity a les permissions

```bash
az keyvault secret show --vault-name geocatis-prod-kv --name SqlConnectionString
```

### Problème : CORS errors

**Solution** : Vérifier la configuration CORS dans appsettings et ajouter l'origine

```json
{
  "Cors": {
    "AllowedOrigins": ["https://votre-domaine.com"]
  }
}
```

### Problème : Application Insights ne reçoit pas de données

**Solution** : Vérifier la connection string

```bash
# Tester localement
export APPLICATIONINSIGHTS_CONNECTION_STRING="your-connection-string"
dotnet run
```

---

## Références

- [ASP.NET Core Configuration](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/)
- [Azure Key Vault](https://docs.microsoft.com/azure/key-vault/)
- [User Secrets](https://docs.microsoft.com/aspnet/core/security/app-secrets)
- [Managed Identity](https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/)
