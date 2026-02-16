# Architecture Application de Gestion de Pharmacie - Côte d'Ivoire

## Vue d'ensemble

Cette documentation décrit l'architecture d'une application moderne de gestion de pharmacie destinée au marché de la Côte d'Ivoire, construite sur Microsoft Azure avec l'écosystème .NET.

## Table des matières

1. [Architecture globale](#architecture-globale)
2. [Services Azure](#services-azure)
3. [Architecture applicative .NET](#architecture-applicative-net)
4. [Sécurité et conformité](#sécurité-et-conformité)
5. [Spécificités pour la Côte d'Ivoire](#spécificités-pour-la-côte-divoire)
6. [Scalabilité et performance](#scalabilité-et-performance)
7. [Monitoring et observabilité](#monitoring-et-observabilité)

---

## Architecture globale

### Vue d'ensemble architecturale

```
┌─────────────────────────────────────────────────────────────────────┐
│                         UTILISATEURS FINAUX                          │
│  (Pharmaciens, Personnel médical, Gestionnaires, Patients)          │
└────────────────────────────┬────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    Azure Front Door + WAF                            │
│              (Protection DDoS, CDN, Load Balancing)                  │
└────────────────────────────┬────────────────────────────────────────┘
                             │
              ┌──────────────┴──────────────┐
              ▼                             ▼
┌──────────────────────────┐  ┌──────────────────────────┐
│   Geocatis.Web (Blazor)  │  │    Geocatis.API          │
│   Azure App Service      │  │    Azure App Service     │
│   (Interface utilisateur)│  │    (Backend REST API)    │
└─────────┬────────────────┘  └─────────┬────────────────┘
          │                             │
          │                             ▼
          │              ┌──────────────────────────────┐
          │              │   Azure API Management       │
          │              │   (Gateway, Throttling,      │
          │              │    Documentation API)        │
          │              └─────────┬────────────────────┘
          │                        │
          └────────────────────────┼─────────────────────┐
                                   │                     │
                    ┌──────────────┴───────┐             │
                    ▼                      ▼             ▼
        ┌────────────────────┐  ┌──────────────────┐ ┌────────────────┐
        │  Azure SQL Database│  │  Azure Cosmos DB │ │ Azure Key Vault│
        │  (Données relation-│  │  (Cache & NoSQL) │ │ (Secrets, Keys)│
        │   nelles)          │  │                  │ │                │
        └────────────────────┘  └──────────────────┘ └────────────────┘
                    │
                    ▼
        ┌────────────────────────────────────────────┐
        │        Azure Blob Storage                  │
        │  (Documents, Images, Rapports)             │
        └────────────────────────────────────────────┘
                    │
                    ▼
        ┌────────────────────────────────────────────┐
        │     Azure Service Bus                      │
        │  (Messaging asynchrone, Intégrations)      │
        └────────────────────────────────────────────┘
                    │
                    ▼
        ┌────────────────────────────────────────────┐
        │    Azure Functions                         │
        │  (Traitements batch, Notifications,        │
        │   Rapports automatisés)                    │
        └────────────────────────────────────────────┘
                    │
                    ▼
        ┌────────────────────────────────────────────┐
        │  Azure Application Insights                │
        │  (Monitoring, Logs, Métriques)             │
        └────────────────────────────────────────────┘
```

### Principes architecturaux

1. **Architecture en couches** : Séparation claire entre présentation, logique métier et données
2. **Microservices légers** : Services découplés et indépendants
3. **Cloud-native** : Exploitation optimale des services Azure PaaS
4. **Sécurité par conception** : Zero Trust, chiffrement, authentification robuste
5. **Scalabilité horizontale** : Capacité à gérer la croissance
6. **Résilience** : Haute disponibilité et reprise après sinistre

---

## Services Azure

### 1. Compute (Calcul)

#### Azure App Service
- **Geocatis.Web** : Application Blazor Server/WebAssembly
  - Plan : Standard S1 minimum (production: Premium P1V2)
  - Auto-scaling basé sur CPU/mémoire
  - Slots de déploiement (dev, staging, production)
  - Always On activé

- **Geocatis.API** : API REST .NET
  - Plan : Standard S1 minimum (production: Premium P1V2)
  - Auto-scaling basé sur le nombre de requêtes
  - Health checks configurés
  - CORS configuré pour Geocatis.Web

#### Azure Functions
- **Traitement asynchrone** :
  - Génération de rapports périodiques
  - Envoi de notifications (SMS, Email)
  - Synchronisation des stocks
  - Alertes de péremption
  - Sauvegarde automatique
- Plan : Consumption ou Premium selon le volume
- Runtime : .NET 8.0+

### 2. Données

#### Azure SQL Database
- **Base de données principale**
  - Tier : Standard S2 minimum (production: Premium P2)
  - Geo-replication pour la haute disponibilité
  - Point-in-time restore activé (35 jours)
  - Transparent Data Encryption (TDE) activé
  - Always Encrypted pour données sensibles
  - Audit activé

**Schéma de données** :
- Médicaments et inventaire
- Patients et prescriptions
- Transactions et facturation
- Utilisateurs et permissions
- Fournisseurs et commandes
- Audit trail

#### Azure Cosmos DB (Optionnel)
- **Cache distribué** et données semi-structurées
  - API : Core (SQL)
  - Mode : Serverless pour démarrage
  - Stockage des sessions utilisateur
  - Cache des requêtes fréquentes
  - Logs d'activité

### 3. Stockage

#### Azure Blob Storage
- **Container principal** :
  - Images de médicaments
  - Documents légaux (ordonnances scannées)
  - Factures PDF
  - Rapports générés
  - Sauvegardes
- Tier : Hot pour accès fréquent, Cool pour archives
- Lifecycle management configuré
- Soft delete activé (14 jours)
- Versioning activé

### 4. Réseau et sécurité

#### Azure Front Door
- Distribution CDN mondiale
- Web Application Firewall (WAF)
- Protection DDoS
- SSL/TLS termination
- Routing intelligent
- Cache global

#### Azure API Management
- Gateway API centralisé
- Gestion des versions API
- Throttling et quotas
- Documentation Swagger/OpenAPI
- Transformation de requêtes
- Monitoring des API
- Developer Portal

#### Azure Key Vault
- Gestion des secrets :
  - Connection strings
  - API keys (SMS, Email, Paiement)
  - Certificats SSL
  - Clés de chiffrement
- Soft delete et purge protection activés
- Audit logging
- Access policies basées sur Managed Identity

### 5. Messaging et événements

#### Azure Service Bus
- **Queues** :
  - Commandes fournisseurs
  - Notifications à envoyer
  - Rapports à générer
  - Synchronisation multi-sites
- **Topics** :
  - Événements métier (vente, stock bas, péremption)
  - Intégration avec systèmes tiers
- Dead letter queue configurée
- Sessions pour traitement ordonné

### 6. Identité et accès

#### Azure Active Directory B2C
- Authentification utilisateurs :
  - Pharmaciens
  - Personnel médical
  - Gestionnaires
  - Patients (portail en ligne)
- Authentification multifacteur (MFA)
- Social login (Google, Microsoft)
- Custom policies pour workflows spécifiques
- Self-service password reset

#### Azure AD (pour le personnel)
- Single Sign-On (SSO)
- Conditional Access
- Privileged Identity Management (PIM)
- Role-Based Access Control (RBAC)

### 7. Monitoring et observabilité

#### Azure Application Insights
- Monitoring application en temps réel
- Télémétrie personnalisée
- Dependency tracking
- Exception tracking
- Performance monitoring
- Live metrics
- Availability tests

#### Azure Monitor
- Logs centralisés
- Métriques infrastructure
- Alertes configurables
- Dashboards personnalisés
- Integration avec Azure Sentinel (SIEM)

#### Azure Log Analytics
- Workspace centralisé
- Requêtes KQL
- Corrélation des logs
- Retention 90 jours minimum

---

## Architecture applicative .NET

### Structure des projets

```
Geocatis.sln
│
├── src/
│   ├── Geocatis.Web/                    # Blazor Frontend
│   │   ├── Pages/                       # Pages Blazor
│   │   ├── Components/                  # Composants réutilisables
│   │   ├── Services/                    # Services client
│   │   └── wwwroot/                     # Assets statiques
│   │
│   ├── Geocatis.API/                    # API REST Backend
│   │   ├── Controllers/                 # API Controllers
│   │   ├── Middleware/                  # Middleware personnalisé
│   │   ├── Filters/                     # Action filters
│   │   └── Extensions/                  # Extensions de services
│   │
│   ├── Geocatis.Core/                   # Logique métier
│   │   ├── Entities/                    # Entités domaine
│   │   ├── Interfaces/                  # Contrats de services
│   │   ├── Services/                    # Services métier
│   │   ├── Validators/                  # FluentValidation
│   │   └── Specifications/              # Spécifications
│   │
│   ├── Geocatis.Data/                   # Accès aux données
│   │   ├── Context/                     # DbContext
│   │   ├── Repositories/                # Repository pattern
│   │   ├── Configurations/              # EF Core configurations
│   │   └── Migrations/                  # Migrations EF
│   │
│   ├── Geocatis.Infrastructure/         # Infrastructure
│   │   ├── Services/                    # Services externes
│   │   ├── Storage/                     # Blob storage
│   │   ├── Messaging/                   # Service Bus
│   │   ├── Email/                       # SendGrid/SMTP
│   │   ├── SMS/                         # Twilio/Africa's Talking
│   │   └── Payment/                     # Orange Money, MTN Mobile Money
│   │
│   └── Geocatis.Shared/                 # Code partagé
│       ├── DTOs/                        # Data Transfer Objects
│       ├── Constants/                   # Constantes
│       ├── Enums/                       # Énumérations
│       └── Extensions/                  # Extensions
│
├── functions/
│   ├── Geocatis.Functions.Reports/      # Génération rapports
│   ├── Geocatis.Functions.Notifications/# Envoi notifications
│   └── Geocatis.Functions.Inventory/    # Gestion inventaire
│
├── tests/
│   ├── Geocatis.UnitTests/              # Tests unitaires
│   ├── Geocatis.IntegrationTests/       # Tests d'intégration
│   └── Geocatis.E2ETests/               # Tests end-to-end
│
└── infrastructure/
    ├── bicep/                           # IaC Bicep templates
    ├── pipelines/                       # Azure DevOps pipelines
    └── scripts/                         # Scripts de déploiement
```

### Technologies et packages .NET

#### Framework
- **.NET 8.0+** : Framework principal
- **ASP.NET Core 8.0+** : Web framework
- **Blazor Server/WASM** : Frontend interactif
- **Entity Framework Core 8.0+** : ORM

#### Packages essentiels

**API & Web**
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.AzureADB2C.UI" />
<PackageReference Include="Swashbuckle.AspNetCore" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
<PackageReference Include="FluentValidation.AspNetCore" />
<PackageReference Include="MediatR" />
```

**Azure SDK**
```xml
<PackageReference Include="Azure.Storage.Blobs" />
<PackageReference Include="Azure.Messaging.ServiceBus" />
<PackageReference Include="Azure.Security.KeyVault.Secrets" />
<PackageReference Include="Azure.Identity" />
<PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" />
```

**Data**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.Azure.Cosmos" />
```

**Sécurité**
```xml
<PackageReference Include="Microsoft.AspNetCore.DataProtection.AzureStorage" />
<PackageReference Include="Microsoft.AspNetCore.DataProtection.AzureKeyVault" />
```

**Utilitaires**
```xml
<PackageReference Include="Serilog.AspNetCore" />
<PackageReference Include="Serilog.Sinks.ApplicationInsights" />
<PackageReference Include="Polly" />
<PackageReference Include="Newtonsoft.Json" />
```

### Patterns et principes

1. **Clean Architecture** : Dépendances vers l'intérieur
2. **CQRS** : Séparation commandes/requêtes (MediatR)
3. **Repository Pattern** : Abstraction accès données
4. **Unit of Work** : Gestion transactions
5. **Dependency Injection** : IoC natif .NET
6. **SOLID Principles** : Code maintenable
7. **Domain-Driven Design** : Modélisation métier

---

## Sécurité et conformité

### Authentification et autorisation

#### Niveaux d'accès
1. **Super Admin** : Gestion complète du système
2. **Gérant de pharmacie** : Gestion opérationnelle
3. **Pharmacien** : Dispensation et conseil
4. **Préparateur** : Gestion stocks et préparations
5. **Caissier** : Ventes et encaissements
6. **Auditeur** : Lecture seule pour audit
7. **Patient** : Accès portail personnel (optionnel)

#### Implémentation
- **Azure AD B2C** pour authentification
- **Claims-based authorization** dans .NET
- **JWT tokens** pour API
- **MFA obligatoire** pour admins
- **Session timeout** : 30 minutes d'inactivité
- **Password policy** : 12 caractères minimum, complexité

### Protection des données

#### Chiffrement
- **At rest** : TDE sur Azure SQL, chiffrement Blob Storage
- **In transit** : TLS 1.3 exclusivement
- **Application level** : Always Encrypted pour données ultra-sensibles

#### Données sensibles
- Informations patient (HIPAA-like)
- Données médicales
- Informations financières
- Identifiants personnels

#### Conformité RGPD
- Consentement explicite
- Droit à l'oubli
- Portabilité des données
- Notification de violation (72h)
- Data Protection Impact Assessment (DPIA)

### Audit et traçabilité

- **Audit logging** complet :
  - Qui a fait quoi, quand, où
  - Accès aux données sensibles
  - Modifications de configuration
  - Transactions financières
- **Retention** : 7 ans minimum (conformité légale)
- **Immutabilité** : Logs en append-only
- **Alertes** : Activités suspectes

### Sécurité réseau

- **Network Security Groups (NSG)**
- **Private Endpoints** pour services Azure
- **Service Endpoints** quand applicable
- **Azure Firewall** pour trafic sortant
- **DDoS Protection Standard**

---

## Spécificités pour la Côte d'Ivoire

### Réglementations locales

#### Conformité pharmaceutique
- **Ordre National des Pharmaciens de Côte d'Ivoire**
- Enregistrement des médicaments
- Traçabilité obligatoire
- Quotas de stupéfiants et psychotropes
- Prescriptions médicales électroniques
- Reporting aux autorités sanitaires

#### Données de santé
- Protection données personnelles (loi ivoirienne)
- Confidentialité médecin-patient
- Conservation dossiers : 20 ans minimum

### Localisation

#### Devise
- **Franc CFA (XOF)** : Devise principale
- Support multi-devises pour import/export
- Gestion TVA : 18% (taux standard CIV)
- Facturation conforme aux normes locales

#### Langue
- **Français** : Langue principale
- Interface utilisateur en français
- Documentation en français
- Support caractères spéciaux français

#### Format de données
- **Date** : Format français (DD/MM/YYYY)
- **Heure** : Format 24h
- **Téléphone** : Format ivoirien (+225)
- **Timezone** : GMT (UTC+0)

### Intégrations locales

#### Paiement mobile
- **Orange Money** (leader en CIV)
- **MTN Mobile Money**
- **Moov Money**
- **Wave**

#### SMS
- **Africa's Talking**
- Opérateurs locaux (Orange, MTN, Moov)

#### Système de santé
- Intégration future avec INHP (Institut National d'Hygiène Publique)
- CMU (Couverture Maladie Universelle)

### Infrastructure réseau

#### Connectivité
- **Azure West Africa** (région la plus proche)
- CDN optimisé pour l'Afrique de l'Ouest
- Mode offline/cache pour zones à faible connectivité
- Optimisation bande passante

#### Performance
- Progressive Web App (PWA) pour accès mobile
- Lazy loading
- Compression gzip/brotli
- Images optimisées

---

## Scalabilité et performance

### Stratégies de scalabilité

#### Scalabilité horizontale
- **App Services** : Auto-scale rules
  - Scale out : > 70% CPU pendant 5 min
  - Scale in : < 30% CPU pendant 10 min
  - Min instances : 2, Max instances : 10

#### Scalabilité verticale
- **Base de données** : Scaling basé sur DTU/vCore
- Upgrade automatique si DTU > 80%

### Optimisations

#### Cache
- **Azure Redis Cache** (optionnel)
- **In-memory cache** (.NET)
- **Output caching** pour pages statiques
- **Response compression**

#### Base de données
- **Indexation** optimale
- **Query optimization**
- **Partitioning** pour tables volumineuses
- **Read replicas** pour lectures intensives

#### Frontend
- **Lazy loading** modules Blazor
- **Virtual scrolling** pour listes longues
- **Debouncing** pour recherches
- **Minification** et bundling

### Haute disponibilité

- **SLA** : 99.95% (App Service Premium)
- **Geo-redundancy** : Réplication inter-régions
- **Health probes** : Checks réguliers
- **Circuit breaker** : Pattern Polly
- **Retry policies** : Exponential backoff

---

## Monitoring et observabilité

### Métriques clés (KPI)

#### Technique
- **Availability** : Uptime
- **Performance** : Response time, throughput
- **Errors** : Error rate, exception count
- **Saturation** : CPU, Memory, Database DTU

#### Métier
- **Nombre de ventes** : Par jour/semaine/mois
- **Valeur stock** : Inventaire actuel
- **Médicaments périmés** : Alertes
- **Taux de rotation** : Produits
- **Marge bénéficiaire** : Par catégorie

### Alertes configurées

1. **Critique** (P1)
   - Application down (> 5 min)
   - Database unavailable
   - Error rate > 5%

2. **Haute** (P2)
   - Response time > 2s
   - CPU > 90% (> 10 min)
   - Memory > 85%
   - Disk space < 10%

3. **Moyenne** (P3)
   - Stock bas (< seuil)
   - Péremption proche (< 30 jours)
   - Backup failed

### Dashboards

1. **Opérationnel** : Santé système temps réel
2. **Métier** : KPIs pharmacie
3. **Sécurité** : Audit et accès
4. **Financier** : Ventes et marges

---

## Déploiement et CI/CD

### Pipeline Azure DevOps

```yaml
# Pipeline example structure
stages:
  - Build
  - Test
  - Deploy to Dev
  - Deploy to Staging
  - Deploy to Production (manual approval)
```

### Stratégie de déploiement

- **Blue-Green deployment** : Zero downtime
- **Staging slots** : Validation avant production
- **Rollback automatique** : Si health check échoue
- **Feature flags** : A/B testing, déploiement progressif

### Environnements

1. **Development** : Intégration continue
2. **Staging** : Tests pré-production
3. **Production** : Environnement live

---

## Coûts estimés (Azure)

### Configuration minimale (démarrage)

| Service | Configuration | Coût mensuel estimé |
|---------|--------------|---------------------|
| App Service (x2) | Standard S1 | ~140 USD |
| Azure SQL Database | Standard S2 | ~150 USD |
| Blob Storage | Standard, 100 GB | ~3 USD |
| Azure Front Door | Standard | ~40 USD |
| Application Insights | 5 GB/mois | ~15 USD |
| Azure Functions | 1M exécutions | ~5 USD |
| Azure Service Bus | Basic | ~10 USD |
| Azure Key Vault | Standard | ~5 USD |
| **TOTAL** | | **~368 USD/mois** |

### Configuration production (moyenne pharmacie)

| Service | Configuration | Coût mensuel estimé |
|---------|--------------|---------------------|
| App Service (x2) | Premium P1V2 | ~340 USD |
| Azure SQL Database | Premium P2 | ~500 USD |
| Blob Storage | Standard, 500 GB | ~12 USD |
| Azure Front Door | Premium + WAF | ~80 USD |
| Application Insights | 15 GB/mois | ~35 USD |
| Azure Functions | 5M exécutions | ~15 USD |
| Azure Service Bus | Standard | ~25 USD |
| Azure Key Vault | Standard | ~5 USD |
| Azure AD B2C | 50k MAU | ~85 USD |
| **TOTAL** | | **~1,097 USD/mois** |

*Note : Coûts indicatifs, variables selon utilisation réelle*

---

## Roadmap et évolutions futures

### Phase 1 (3 mois) : MVP
- Gestion de base des médicaments
- Point de vente simple
- Gestion des stocks
- Utilisateurs et permissions

### Phase 2 (6 mois) : Fonctionnalités avancées
- Gestion des prescriptions
- Intégration paiements mobiles
- Rapports avancés
- Application mobile (Xamarin/MAUI)

### Phase 3 (12 mois) : Intelligence et intégration
- IA pour prévision stocks
- Reconnaissance d'images (ordonnances)
- Intégration système de santé national
- Plateforme multi-pharmacies

### Phase 4 (18 mois+) : Expansion
- E-commerce (vente en ligne)
- Livraison à domicile
- Téléconsultation
- Expansion régionale (CEDEAO)

---

## Conclusion

Cette architecture propose une solution moderne, sécurisée et scalable pour la gestion de pharmacie en Côte d'Ivoire. Elle exploite pleinement l'écosystème Azure et .NET pour offrir une plateforme robuste, conforme aux réglementations locales et capable d'évoluer avec les besoins métier.

### Points clés
✅ Architecture cloud-native sur Azure  
✅ Stack .NET moderne et performante  
✅ Sécurité et conformité RGPD  
✅ Adaptation au contexte ivoirien  
✅ Scalabilité et haute disponibilité  
✅ Monitoring et observabilité complète  

### Prochaines étapes recommandées
1. Validation de l'architecture avec les stakeholders
2. Proof of Concept (PoC) sur un périmètre restreint
3. Développement itératif (Agile/Scrum)
4. Formation des utilisateurs
5. Déploiement progressif (pilote puis généralisation)
