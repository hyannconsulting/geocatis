# Geocatis - Application de Gestion de Pharmacie

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-Cloud-0078D4?logo=microsoftazure)](https://azure.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 📋 Vue d'ensemble

Geocatis est une application moderne de gestion de pharmacie conçue spécifiquement pour le marché de la Côte d'Ivoire (CIV) et de l'Afrique de l'Ouest. L'application est construite sur **Microsoft Azure** avec l'écosystème **.NET** pour offrir une solution robuste, sécurisée et scalable.

### 🎯 Objectifs

- **Gestion complète** : Stocks, ventes, prescriptions, patients, fournisseurs
- **Conformité** : Respect des réglementations ivoiriennes et internationales (RGPD)
- **Cloud-native** : Architecture Azure pour haute disponibilité et performance
- **Localisation** : Adaptation complète au marché CIV (Franc CFA, français, paiements mobiles)
- **Sécurité** : Chiffrement, audit, traçabilité complète

### ✨ Fonctionnalités principales

#### Gestion des médicaments
- 📦 Catalogue complet avec codes CIP/EAN
- 🏷️ Prix d'achat/vente, TVA, remises
- 📊 Gestion multi-lots avec FEFO (First Expired, First Out)
- ⚠️ Alertes de stock bas et péremption
- 🔍 Recherche avancée et filtres

#### Point de vente
- 💰 Encaissement multi-modes (espèces, mobile money, cartes)
- 📝 Support des prescriptions médicales
- 🧾 Facturation conforme aux normes locales
- 🎁 Système de remises et promotions
- 📱 Interface tactile optimisée

#### Gestion des stocks
- 📥 Commandes fournisseurs automatisées
- 📋 Inventaires périodiques avec écarts
- 🔄 Synchronisation temps réel
- 📈 Prévisions et réapprovisionnement intelligent
- 🚨 Traçabilité complète (lots, péremptions)

#### Patients et prescriptions
- 👥 Dossier patient avec historique
- 💊 Gestion des ordonnances électroniques
- ⚕️ Alertes interactions médicamenteuses
- 🏥 Support CMU (Couverture Maladie Universelle)
- 🔒 Confidentialité et sécurité des données

#### Reporting et analytics
- 📊 Tableaux de bord en temps réel
- 📈 Analyses de ventes et marges
- 📉 Rapports de stock et péremption
- 💹 Business Intelligence
- 📧 Rapports automatisés par email

## 🏗️ Architecture

### Architecture technique

L'application suit une architecture **Clean Architecture** avec séparation des préoccupations :

```
┌─────────────────────────────────────────────────┐
│           Frontend (Blazor WebAssembly)         │
│              Geocatis.Web                       │
└──────────────────┬──────────────────────────────┘
                   │ HTTPS/REST
                   ▼
┌─────────────────────────────────────────────────┐
│           API Gateway (APIM)                     │
└──────────────────┬──────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────┐
│           Backend API (.NET 8)                   │
│              Geocatis.API                        │
│  ┌──────────────────────────────────────────┐   │
│  │        Geocatis.Core (Business)          │   │
│  └──────────────────┬───────────────────────┘   │
│                     │                            │
│  ┌──────────────────▼───────────────────────┐   │
│  │      Geocatis.Data (Repository)          │   │
│  └──────────────────┬───────────────────────┘   │
└───────────────────┬─┴───────────────────────────┘
                    │
        ┌───────────┴───────────┐
        ▼                       ▼
┌──────────────────┐   ┌──────────────────┐
│  Azure SQL       │   │  Blob Storage    │
│  Database        │   │  (Documents)     │
└──────────────────┘   └──────────────────┘
```

### Services Azure utilisés

| Service | Usage | Tier |
|---------|-------|------|
| **Azure App Service** | Hébergement Web & API | Standard/Premium |
| **Azure SQL Database** | Base de données principale | Standard S2/Premium P2 |
| **Azure Blob Storage** | Documents, images, rapports | Standard |
| **Azure Key Vault** | Gestion des secrets | Standard |
| **Azure Service Bus** | Messaging asynchrone | Basic/Standard |
| **Azure Functions** | Traitements batch | Consumption |
| **Application Insights** | Monitoring & logs | Pay-as-you-go |
| **Azure Front Door** | CDN & WAF | Standard/Premium |
| **Azure API Management** | Gateway API | Developer/Standard |

Pour plus de détails, consultez [ARCHITECTURE.md](docs/ARCHITECTURE.md).

## 🚀 Démarrage rapide

### Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou supérieur
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (pour déploiement)
- [SQL Server](https://www.microsoft.com/sql-server) ou Azure SQL Database
- Compte Azure (pour déploiement cloud)

### Installation locale

1. **Cloner le repository**
```bash
git clone https://github.com/hyannconsulting/geocatis.git
cd geocatis
```

2. **Restaurer les dépendances**
```bash
dotnet restore
```

3. **Configurer la base de données**
```bash
cd Geocatis.Data
dotnet ef database update
```

4. **Configurer les secrets**
```bash
cd ../Geocatis.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GeocatisDB;Trusted_Connection=True;"
```

5. **Lancer l'application**
```bash
# Terminal 1 - API
cd Geocatis.API
dotnet run

# Terminal 2 - Web
cd Geocatis.Web
dotnet run
```

6. **Accéder à l'application**
- Web : https://localhost:5001
- API : https://localhost:7001
- Swagger : https://localhost:7001/swagger

## 📦 Structure du projet

```
Geocatis/
├── Geocatis.API/              # API REST Backend
├── Geocatis.Web/              # Frontend Blazor
├── Geocatis.Core/             # Logique métier (à créer)
├── Geocatis.Data/             # Accès aux données
├── Geocatis.Shared/           # DTOs et modèles partagés (à créer)
├── Geocatis.Infrastructure/   # Services externes (à créer)
├── docs/                      # Documentation
│   ├── ARCHITECTURE.md        # Architecture détaillée
│   └── DOMAIN-MODEL.md        # Modèle de domaine
├── infrastructure/            # IaC et déploiement
│   └── bicep/                 # Templates Azure Bicep
├── tests/                     # Tests (à créer)
└── README.md                  # Ce fichier
```

## 🌍 Spécificités Côte d'Ivoire

### Conformité réglementaire
- ✅ Ordre National des Pharmaciens de Côte d'Ivoire
- ✅ Traçabilité des médicaments (stupéfiants, psychotropes)
- ✅ Conservation légale : 7 ans minimum
- ✅ Facturation conforme (TVA 18%)

### Localisation
- 🇫🇷 Interface en français
- 💰 Franc CFA (XOF)
- 📞 Format téléphone ivoirien (+225)
- 🌍 Timezone GMT (UTC+0)

### Intégrations locales
- 📱 **Orange Money** (paiement mobile #1 en CIV)
- 📱 **MTN Mobile Money**
- 📱 **Moov Money**
- 📱 **Wave**
- 💬 **Africa's Talking** (SMS)

## 🔐 Sécurité

### Authentification
- Azure Active Directory B2C
- Authentification multifacteur (MFA)
- JWT tokens pour API
- Role-Based Access Control (RBAC)

### Chiffrement
- TLS 1.3 pour transit
- Transparent Data Encryption (TDE) au repos
- Always Encrypted pour données ultra-sensibles
- Azure Key Vault pour secrets

### Conformité
- RGPD : Consentement, droit à l'oubli, portabilité
- Audit trail complet
- Logs immutables (7 ans)
- Sécurité par conception

## 📊 Monitoring

### Métriques surveillées
- ⏱️ Performance : Response time, throughput
- 💾 Ressources : CPU, Memory, Database DTU
- 🐛 Errors : Exception rate, failed requests
- 📈 Métier : Ventes, stocks, alertes

### Outils
- Application Insights : Télémétrie applicative
- Azure Monitor : Infrastructure et logs
- Log Analytics : Analyse et requêtes
- Dashboards personnalisés

## 🚢 Déploiement

### Déploiement Azure

1. **Créer le groupe de ressources**
```bash
az group create --name geocatis-prod-rg --location westeurope
```

2. **Déployer l'infrastructure**
```bash
cd infrastructure/bicep
az deployment group create \
  --resource-group geocatis-prod-rg \
  --template-file main.bicep \
  --parameters main.parameters.prod.json
```

3. **Déployer l'application**
```bash
# Via Azure DevOps, GitHub Actions, ou manuellement
dotnet publish -c Release
az webapp deployment source config-zip --src publish.zip
```

Consultez [infrastructure/bicep/README.md](infrastructure/bicep/README.md) pour plus de détails.

## 🧪 Tests

```bash
# Tests unitaires
dotnet test Geocatis.UnitTests

# Tests d'intégration
dotnet test Geocatis.IntegrationTests

# Tests E2E
dotnet test Geocatis.E2ETests
```

## 📖 Documentation

- [Architecture complète](docs/ARCHITECTURE.md)
- [Modèle de domaine](docs/DOMAIN-MODEL.md)
- [Pattern CQRS](docs/CQRS.md)
- [Guide de déploiement](infrastructure/bicep/README.md)
- [Instructions pour Copilot](.github/copilot-instructions.md)

## 🛣️ Roadmap

### Phase 1 - MVP (3 mois) ✅
- [x] Architecture et infrastructure Azure
- [ ] Gestion basique des médicaments
- [ ] Point de vente simple
- [ ] Gestion des stocks
- [ ] Utilisateurs et permissions

### Phase 2 - Fonctionnalités avancées (6 mois)
- [ ] Gestion des prescriptions
- [ ] Intégration paiements mobiles
- [ ] Rapports avancés
- [ ] Application mobile (MAUI)

### Phase 3 - Intelligence (12 mois)
- [ ] IA pour prévision stocks
- [ ] Reconnaissance d'images (ordonnances OCR)
- [ ] Intégration système de santé national
- [ ] Plateforme multi-pharmacies

### Phase 4 - Expansion (18 mois+)
- [ ] E-commerce (vente en ligne)
- [ ] Livraison à domicile
- [ ] Téléconsultation
- [ ] Expansion régionale CEDEAO

## 💰 Coûts estimés Azure

### Environnement de développement
~**368 USD/mois**

### Environnement de production
~**1,097 USD/mois**

Détails dans [ARCHITECTURE.md](docs/ARCHITECTURE.md#coûts-estimés-azure)

## 🤝 Contribution

Les contributions sont les bienvenues ! Consultez notre guide de contribution (à venir).

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit vos changements (`git commit -m 'Add AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## 📝 License

Ce projet est sous licence MIT. Voir [LICENSE](LICENSE) pour plus de détails.

## 👥 Équipe

**Hyann Consulting** - Expertise IT et solutions Cloud

## 📧 Contact

- Website : [https://hyannconsulting.com](https://hyannconsulting.com)
- Email : contact@hyannconsulting.com
- GitHub : [@hyannconsulting](https://github.com/hyannconsulting)

## 🙏 Remerciements

- Microsoft Azure pour la plateforme cloud
- Communauté .NET pour les outils et frameworks
- Ordre National des Pharmaciens de Côte d'Ivoire pour les guidelines

---

**Fait avec ❤️ pour les pharmaciens de Côte d'Ivoire et d'Afrique de l'Ouest**
