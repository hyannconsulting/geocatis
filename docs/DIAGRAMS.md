# Diagrammes et Visualisations - Geocatis

Ce document contient les diagrammes architecturaux et visuels de l'application Geocatis.

## Architecture globale Azure

```mermaid
graph TB
    subgraph "Utilisateurs"
        U1[👨‍⚕️ Pharmaciens]
        U2[👥 Patients]
        U3[📱 Mobile]
    end

    subgraph "Azure Front End"
        AFD[Azure Front Door + WAF]
        CDN[CDN Global]
    end

    subgraph "Application Layer"
        WEB[Blazor WebAssembly<br/>Geocatis.Web]
        API[ASP.NET Core API<br/>Geocatis.API]
        APIM[API Management]
    end

    subgraph "Business Logic"
        CORE[Geocatis.Core<br/>Business Logic]
        INFRA[Geocatis.Infrastructure<br/>External Services]
    end

    subgraph "Data Layer"
        SQL[(Azure SQL Database<br/>Main Database)]
        BLOB[Blob Storage<br/>Documents/Images]
        COSMOS[(Cosmos DB<br/>Cache/NoSQL)]
    end

    subgraph "Integration Layer"
        SB[Service Bus<br/>Messaging]
        FUNC[Azure Functions<br/>Background Jobs]
    end

    subgraph "Security & Monitoring"
        KV[Key Vault<br/>Secrets]
        ADB2C[Azure AD B2C<br/>Authentication]
        AI[Application Insights<br/>Monitoring]
    end

    U1 --> AFD
    U2 --> AFD
    U3 --> AFD
    
    AFD --> CDN
    CDN --> WEB
    CDN --> APIM
    
    WEB --> APIM
    APIM --> API
    
    API --> CORE
    CORE --> INFRA
    
    API --> SQL
    API --> BLOB
    API --> COSMOS
    
    API --> SB
    SB --> FUNC
    
    API --> KV
    API --> ADB2C
    API --> AI
    
    FUNC --> SQL
    FUNC --> BLOB
    FUNC --> AI

    style AFD fill:#0078D4
    style WEB fill:#512BD4
    style API fill:#512BD4
    style SQL fill:#CC2927
    style KV fill:#FDB813
    style AI fill:#68217A
```

## Architecture applicative .NET

```mermaid
graph LR
    subgraph "Presentation Layer"
        BLAZOR[Blazor Components<br/>Pages & UI]
    end

    subgraph "API Layer"
        CONTROLLERS[Controllers<br/>REST Endpoints]
        MIDDLEWARE[Middleware<br/>Auth, Logging, etc.]
    end

    subgraph "Application Layer"
        SERVICES[Application Services<br/>Use Cases]
        VALIDATORS[Validators<br/>FluentValidation]
        MAPPERS[AutoMapper<br/>DTOs]
    end

    subgraph "Domain Layer"
        ENTITIES[Domain Entities<br/>Business Models]
        INTERFACES[Interfaces<br/>Contracts]
        RULES[Business Rules<br/>Domain Logic]
    end

    subgraph "Infrastructure Layer"
        REPOS[Repositories<br/>Data Access]
        DBCONTEXT[EF Core<br/>DbContext]
        EXTERNAL[External Services<br/>SMS, Email, Payment]
    end

    subgraph "Data Store"
        DB[(Database)]
        STORAGE[(Storage)]
    end

    BLAZOR --> CONTROLLERS
    CONTROLLERS --> MIDDLEWARE
    MIDDLEWARE --> SERVICES
    SERVICES --> VALIDATORS
    SERVICES --> MAPPERS
    SERVICES --> INTERFACES
    INTERFACES --> ENTITIES
    ENTITIES --> RULES
    INTERFACES --> REPOS
    REPOS --> DBCONTEXT
    DBCONTEXT --> DB
    SERVICES --> EXTERNAL
    EXTERNAL --> STORAGE

    style BLAZOR fill:#512BD4
    style ENTITIES fill:#68B984
    style REPOS fill:#FFB900
    style DB fill:#CC2927
```

## Flux de vente

```mermaid
sequenceDiagram
    actor P as Pharmacien
    participant UI as Blazor UI
    participant API as API Backend
    participant BL as Business Logic
    participant DB as Database
    participant SB as Service Bus
    participant FUNC as Functions

    P->>UI: Scanner médicament
    UI->>API: GET /api/medicaments/{code}
    API->>BL: GetMedicamentByCode()
    BL->>DB: Query médicament + stock
    DB-->>BL: Données médicament
    BL-->>API: MedicamentDTO
    API-->>UI: Médicament info
    UI-->>P: Afficher infos

    P->>UI: Ajouter au panier
    UI->>UI: Calculer total

    P->>UI: Valider vente
    UI->>API: POST /api/ventes
    API->>BL: CreateVente()
    
    alt Stock suffisant
        BL->>DB: Begin Transaction
        BL->>DB: Insert Vente
        BL->>DB: Insert LignesVente
        BL->>DB: Update Stock (FEFO)
        BL->>DB: Commit Transaction
        DB-->>BL: Success
        
        BL->>SB: Publish VenteCreatedEvent
        SB-->>FUNC: Trigger notification
        FUNC->>P: SMS/Email confirmation
        
        BL-->>API: VenteDTO
        API-->>UI: Success
        UI-->>P: Imprimer facture
    else Stock insuffisant
        BL-->>API: Error: Stock insuffisant
        API-->>UI: Error 400
        UI-->>P: Alerte stock
    end
```

## Modèle de données (Relations principales)

```mermaid
erDiagram
    PHARMACIE ||--o{ UTILISATEUR : emploie
    PHARMACIE ||--o{ VENTE : effectue
    PHARMACIE ||--o{ STOCK : gere
    
    UTILISATEUR ||--o{ VENTE : cree
    UTILISATEUR ||--o{ COMMANDE : passe
    
    MEDICAMENT ||--o{ STOCK : compose
    MEDICAMENT ||--o{ LIGNE_VENTE : vendu_dans
    MEDICAMENT ||--o{ LIGNE_COMMANDE : commande_dans
    
    VENTE ||--o{ LIGNE_VENTE : contient
    VENTE }o--|| PATIENT : concerne
    VENTE }o--o| PRESCRIPTION : base_sur
    
    COMMANDE ||--o{ LIGNE_COMMANDE : contient
    COMMANDE }o--|| FOURNISSEUR : provient_de
    
    PATIENT ||--o{ PRESCRIPTION : possede
    PATIENT ||--o{ VENTE : achete
    
    INVENTAIRE ||--o{ LIGNE_INVENTAIRE : contient
    LIGNE_INVENTAIRE }o--|| MEDICAMENT : compte

    PHARMACIE {
        int Id PK
        string Nom
        string NumeroAutorisation
        string Telephone
        bool EstActive
    }
    
    MEDICAMENT {
        int Id PK
        string CodeCIP
        string Nom
        string DCI
        decimal PrixVente
        int StockActuel
        bool EstStupefiant
    }
    
    VENTE {
        int Id PK
        string NumeroVente
        datetime DateVente
        decimal MontantTTC
        string ModePaiement
        string Statut
    }
    
    STOCK {
        int Id PK
        string NumeroLot
        int Quantite
        datetime DatePeremption
        decimal PrixAchatUnitaire
    }
    
    PATIENT {
        int Id PK
        string Nom
        string Prenom
        string NumeroCMU
        string Allergies
    }
```

## Flux d'authentification Azure AD B2C

```mermaid
sequenceDiagram
    actor U as Utilisateur
    participant WEB as Blazor Web
    participant ADB2C as Azure AD B2C
    participant API as API Backend
    participant KV as Key Vault

    U->>WEB: Accéder à l'app
    WEB->>ADB2C: Redirect vers login
    ADB2C-->>U: Page de connexion
    U->>ADB2C: Credentials + MFA
    
    alt Authentication Success
        ADB2C->>ADB2C: Valider credentials
        ADB2C->>ADB2C: Vérifier MFA
        ADB2C-->>WEB: ID Token + Access Token
        WEB->>API: Request avec Bearer Token
        API->>API: Valider JWT
        API->>KV: Get signing keys
        KV-->>API: Public keys
        API->>API: Vérifier signature
        API-->>WEB: Response + Data
        WEB-->>U: Afficher dashboard
    else Authentication Failed
        ADB2C-->>U: Erreur authentification
        U->>ADB2C: Réessayer ou reset password
    end
```

## Architecture de monitoring

```mermaid
graph TB
    subgraph "Applications"
        APP1[Geocatis.Web]
        APP2[Geocatis.API]
        APP3[Functions]
    end

    subgraph "Azure Monitor"
        AI[Application Insights<br/>Telemetry]
        LA[Log Analytics<br/>Workspace]
        METRICS[Azure Metrics]
    end

    subgraph "Alerting"
        ALERTS[Alert Rules]
        AG[Action Groups]
        EMAIL[Email]
        SMS[SMS]
        WEBHOOK[Webhook]
    end

    subgraph "Visualization"
        DASH[Azure Dashboards]
        WB[Workbooks]
        PWRBI[Power BI]
    end

    APP1 --> AI
    APP2 --> AI
    APP3 --> AI
    
    AI --> LA
    METRICS --> LA
    
    LA --> ALERTS
    ALERTS --> AG
    
    AG --> EMAIL
    AG --> SMS
    AG --> WEBHOOK
    
    LA --> DASH
    LA --> WB
    LA --> PWRBI

    style AI fill:#68217A
    style LA fill:#0078D4
    style ALERTS fill:#FF6347
```

## Déploiement CI/CD

```mermaid
graph LR
    subgraph "Development"
        DEV[👨‍💻 Developer]
        GIT[Git Repository]
    end

    subgraph "Azure DevOps / GitHub Actions"
        TRIGGER[Trigger]
        BUILD[Build .NET]
        TEST[Run Tests]
        SCAN[Security Scan]
        PACKAGE[Create Artifacts]
    end

    subgraph "Staging"
        DEPLOY_STG[Deploy to Staging]
        TEST_STG[Integration Tests]
        APPROVE[Manual Approval]
    end

    subgraph "Production"
        DEPLOY_PROD[Deploy to Production<br/>Blue-Green]
        SMOKE[Smoke Tests]
        SWITCH[Switch Traffic]
        ROLLBACK[Rollback if needed]
    end

    DEV -->|Push| GIT
    GIT -->|Webhook| TRIGGER
    TRIGGER --> BUILD
    BUILD --> TEST
    TEST --> SCAN
    SCAN --> PACKAGE
    PACKAGE --> DEPLOY_STG
    DEPLOY_STG --> TEST_STG
    TEST_STG --> APPROVE
    APPROVE --> DEPLOY_PROD
    DEPLOY_PROD --> SMOKE
    SMOKE -->|Success| SWITCH
    SMOKE -->|Failure| ROLLBACK

    style BUILD fill:#512BD4
    style TEST fill:#68B984
    style SCAN fill:#FF6347
    style APPROVE fill:#FFB900
    style SWITCH fill:#00A4EF
```

## Intégrations de paiement mobile

```mermaid
graph TB
    subgraph "Geocatis API"
        PAYMENT[Payment Service]
        FACTORY[Payment Provider Factory]
    end

    subgraph "Payment Providers"
        OM[Orange Money<br/>Provider]
        MTN[MTN Mobile Money<br/>Provider]
        MOOV[Moov Money<br/>Provider]
        WAVE[Wave<br/>Provider]
    end

    subgraph "External APIs"
        OM_API[Orange Money API]
        MTN_API[MTN API]
        MOOV_API[Moov API]
        WAVE_API[Wave API]
    end

    subgraph "Storage"
        DB[(Database<br/>Transactions)]
        SB[Service Bus<br/>Payment Events]
    end

    PAYMENT --> FACTORY
    FACTORY --> OM
    FACTORY --> MTN
    FACTORY --> MOOV
    FACTORY --> WAVE

    OM --> OM_API
    MTN --> MTN_API
    MOOV --> MOOV_API
    WAVE --> WAVE_API

    PAYMENT --> DB
    PAYMENT --> SB

    style PAYMENT fill:#512BD4
    style OM fill:#FF6600
    style MTN fill:#FFCC00
    style MOOV fill:#009FE3
    style WAVE fill:#7B61FF
```

## Sécurité en couches

```mermaid
graph TB
    subgraph "Network Security"
        AFD[Azure Front Door<br/>+ WAF]
        NSG[Network Security Groups]
        PE[Private Endpoints]
    end

    subgraph "Application Security"
        AUTH[Azure AD B2C<br/>Authentication]
        AUTHZ[Authorization<br/>RBAC]
        TLS[TLS 1.3]
    end

    subgraph "Data Security"
        KV[Key Vault<br/>Secrets Management]
        TDE[Transparent Data<br/>Encryption]
        AE[Always Encrypted<br/>Sensitive Data]
    end

    subgraph "Monitoring & Compliance"
        AUDIT[Audit Logs]
        THREAT[Threat Detection]
        COMPLIANCE[Compliance Reports]
    end

    AFD --> NSG
    NSG --> PE
    PE --> AUTH
    AUTH --> AUTHZ
    AUTHZ --> TLS
    TLS --> KV
    KV --> TDE
    TDE --> AE
    AE --> AUDIT
    AUDIT --> THREAT
    THREAT --> COMPLIANCE

    style AFD fill:#0078D4
    style AUTH fill:#00A4EF
    style KV fill:#FDB813
    style TDE fill:#CC2927
    style AUDIT fill:#68217A
```

---

## Légende des couleurs

- 🔵 **Bleu Azure** (#0078D4) : Services Azure Platform
- 🟣 **Violet .NET** (#512BD4) : Applications .NET
- 🟢 **Vert** (#68B984) : Logique métier / Success
- 🔴 **Rouge** (#CC2927) : Base de données / Errors
- 🟡 **Jaune** (#FFB900) : Storage / Warnings
- 🟠 **Orange** (#FF6600) : Intégrations externes

---

## Notes

Ces diagrammes sont générés avec Mermaid et peuvent être visualisés directement dans GitHub, VS Code, ou tout outil compatible Mermaid.

Pour générer des diagrammes modifiables ou des exports PNG/SVG, utilisez :
- [Mermaid Live Editor](https://mermaid.live/)
- Extensions VS Code : Mermaid Preview
- draw.io pour diagrammes plus détaillés
