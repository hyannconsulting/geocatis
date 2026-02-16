# Guide de démarrage rapide - Geocatis Pharmacy Management

Bienvenue ! Ce guide vous aidera à démarrer rapidement avec l'application Geocatis de gestion de pharmacie.

## 📚 Table des matières

1. [Présentation](#présentation)
2. [Documentation disponible](#documentation-disponible)
3. [Démarrage rapide - Développement](#démarrage-rapide---développement)
4. [Démarrage rapide - Production](#démarrage-rapide---production)
5. [Prochaines étapes](#prochaines-étapes)

---

## Présentation

Geocatis est une application complète de gestion de pharmacie conçue pour le marché de la Côte d'Ivoire et de l'Afrique de l'Ouest. Elle utilise :

- ☁️ **Microsoft Azure** pour l'infrastructure cloud
- 💻 **. NET 8** pour le backend
- ⚡ **Blazor** pour l'interface utilisateur
- 🗄️ **Azure SQL Database** pour les données
- 🔐 **Azure AD B2C** pour l'authentification

---

## Documentation disponible

### 📖 Documentation d'architecture

| Document | Description | Chemin |
|----------|-------------|--------|
| **Architecture complète** | Architecture Azure, services, patterns | [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) |
| **Modèle de domaine** | Entités métier, relations, règles | [`docs/DOMAIN-MODEL.md`](docs/DOMAIN-MODEL.md) |
| **Diagrammes** | Visualisations Mermaid | [`docs/DIAGRAMS.md`](docs/DIAGRAMS.md) |
| **Configuration** | Guide de configuration | [`docs/CONFIGURATION.md`](docs/CONFIGURATION.md) |

### 🏗️ Infrastructure as Code

| Fichier | Description |
|---------|-------------|
| `infrastructure/bicep/main.bicep` | Template principal Azure |
| `infrastructure/bicep/main.parameters.dev.json` | Paramètres développement |
| `infrastructure/bicep/main.parameters.prod.json` | Paramètres production |
| `infrastructure/bicep/README.md` | Guide de déploiement |

### 📋 Documentation projet

| Fichier | Description |
|---------|-------------|
| `README.md` | Documentation principale du projet |
| `LICENSE` | Licence MIT |

---

## Démarrage rapide - Développement

### 1️⃣ Prérequis

Installer les outils suivants :

```bash
# .NET SDK 8.0
winget install Microsoft.DotNet.SDK.8

# Visual Studio 2022 ou VS Code
winget install Microsoft.VisualStudio.2022.Community

# Azure CLI (pour déploiement)
winget install Microsoft.AzureCLI

# SQL Server (local) ou Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2️⃣ Cloner et configurer

```bash
# Cloner le repository
git clone https://github.com/hyannconsulting/geocatis.git
cd geocatis

# Restaurer les packages
dotnet restore

# Configurer les secrets (développement)
cd Geocatis.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Database=GeocatisDB;Trusted_Connection=True;"
```

### 3️⃣ Créer la base de données

```bash
# Migrations Entity Framework (à venir)
cd Geocatis.Data
dotnet ef database update
```

### 4️⃣ Lancer l'application

```bash
# Terminal 1 - API Backend
cd Geocatis.API
dotnet run
# Écoute sur https://localhost:7001

# Terminal 2 - Frontend Blazor
cd Geocatis.Web
dotnet run
# Écoute sur https://localhost:5001
```

### 5️⃣ Accéder à l'application

- 🌐 **Web App** : https://localhost:5001
- 🔌 **API** : https://localhost:7001
- 📖 **Swagger** : https://localhost:7001/swagger

---

## Démarrage rapide - Production

### 1️⃣ Prérequis Azure

- Compte Azure actif
- Permissions pour créer des ressources
- Azure CLI configuré

```bash
# Se connecter à Azure
az login

# Définir l'abonnement
az account set --subscription "Votre-Subscription-ID"
```

### 2️⃣ Déployer l'infrastructure

```bash
# Créer le groupe de ressources
az group create \
  --name geocatis-prod-rg \
  --location westeurope

# Déployer avec Bicep
cd infrastructure/bicep
az deployment group create \
  --resource-group geocatis-prod-rg \
  --template-file main.bicep \
  --parameters main.parameters.prod.json
```

### 3️⃣ Configurer les secrets

```bash
# Ajouter les secrets dans Key Vault
az keyvault secret set \
  --vault-name geocatis-prod-kv \
  --name SqlConnectionString \
  --value "Votre-Connection-String"

az keyvault secret set \
  --vault-name geocatis-prod-kv \
  --name OrangeMoneyApiKey \
  --value "Votre-API-Key"
```

### 4️⃣ Déployer l'application

```bash
# Build et publish
dotnet publish Geocatis.API -c Release -o ./publish/api
dotnet publish Geocatis.Web -c Release -o ./publish/web

# Déployer vers App Service
az webapp deployment source config-zip \
  --resource-group geocatis-prod-rg \
  --name geocatis-prod-api \
  --src ./publish/api.zip

az webapp deployment source config-zip \
  --resource-group geocatis-prod-rg \
  --name geocatis-prod-web \
  --src ./publish/web.zip
```

### 5️⃣ Vérifier le déploiement

```bash
# Obtenir les URLs
az deployment group show \
  --resource-group geocatis-prod-rg \
  --name geocatis-deployment \
  --query properties.outputs
```

Visiter les URLs pour vérifier que l'application fonctionne.

---

## Prochaines étapes

### 🎯 Phase 1 - Développement MVP (3 mois)

1. **Semaine 1-2 : Setup & Infrastructure**
   - ✅ Architecture documentée
   - ✅ Infrastructure Azure prête
   - ⬜ CI/CD pipeline (Azure DevOps ou GitHub Actions)
   - ⬜ Environnements dev/staging/prod

2. **Semaine 3-6 : Entités et Base de données**
   - ⬜ Créer le projet Geocatis.Core
   - ⬜ Implémenter les entités du domaine
   - ⬜ Configurer Entity Framework Core
   - ⬜ Créer les migrations initiales
   - ⬜ Seed data pour développement

3. **Semaine 7-10 : API Backend**
   - ⬜ Controllers CRUD pour médicaments
   - ⬜ Gestion des stocks (FEFO)
   - ⬜ Point de vente et encaissement
   - ⬜ Gestion des patients
   - ⬜ Authentication & Authorization
   - ⬜ Tests unitaires

4. **Semaine 11-12 : Frontend Blazor**
   - ⬜ Layout et navigation
   - ⬜ Pages de gestion médicaments
   - ⬜ Interface point de vente
   - ⬜ Tableau de bord
   - ⬜ Rapports basiques

### 📊 Métriques de succès

- ✅ **Architecture** : Documentée et validée
- ⬜ **Code Coverage** : > 80% des tests
- ⬜ **Performance** : Response time < 500ms
- ⬜ **Sécurité** : Zéro vulnérabilité critique
- ⬜ **Uptime** : > 99.9%

### 🎓 Formation nécessaire

1. **Développeurs**
   - .NET 8 et C# moderne
   - Entity Framework Core
   - Blazor WebAssembly
   - Azure services (SQL, Storage, Key Vault)
   - Clean Architecture patterns

2. **DevOps**
   - Azure CLI et Bicep
   - CI/CD avec Azure DevOps
   - Monitoring avec Application Insights
   - Sécurité Azure

3. **Utilisateurs finaux**
   - Formation à l'interface
   - Processus métier
   - Support et maintenance

---

## 🆘 Support et aide

### Documentation
- 📖 [Architecture complète](docs/ARCHITECTURE.md)
- 🏗️ [Modèle de domaine](docs/DOMAIN-MODEL.md)
- ⚙️ [Configuration](docs/CONFIGURATION.md)
- 📊 [Diagrammes](docs/DIAGRAMS.md)

### Ressources externes
- [Documentation .NET](https://docs.microsoft.com/dotnet/)
- [Documentation Azure](https://docs.microsoft.com/azure/)
- [Blazor](https://blazor.net/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

### Communauté
- GitHub Issues : [github.com/hyannconsulting/geocatis/issues](https://github.com/hyannconsulting/geocatis/issues)
- Email : contact@hyannconsulting.com

---

## ✅ Checklist avant de commencer

- [ ] Lire `README.md` principal
- [ ] Consulter `docs/ARCHITECTURE.md` pour comprendre l'architecture
- [ ] Parcourir `docs/DOMAIN-MODEL.md` pour le modèle métier
- [ ] Installer tous les prérequis
- [ ] Cloner le repository
- [ ] Configurer l'environnement local
- [ ] Lancer l'application en dev
- [ ] Créer une branche feature pour vos développements

---

**Bon développement ! 🚀**

Pour toute question, n'hésitez pas à ouvrir une issue ou contacter l'équipe.

---

*Document créé le 16 février 2026 pour le projet Geocatis Pharmacy Management*
