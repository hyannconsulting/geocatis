# CQRS — Command Query Responsibility Segregation

## Table des matières

1. [Définition](#définition)
2. [Motivations](#motivations)
3. [Principes : séparation lectures / écritures](#principes--séparation-lectures--écritures)
4. [Implications sur les modèles et les données](#implications-sur-les-modèles-et-les-données)
5. [Avantages et inconvénients](#avantages-et-inconvénients)
6. [Quand utiliser CQRS ?](#quand-utiliser-cqrs-)
7. [Lien avec Event Sourcing](#lien-avec-event-sourcing)
8. [Exemple — API Pharmacie (Geocatis)](#exemple--api-pharmacie-geocatis)

---

## Définition

**CQRS** (Command Query Responsibility Segregation) est un pattern architectural qui consiste à **séparer les opérations d'écriture (Commands) des opérations de lecture (Queries)** au sein d'une même application.

Il est fondé sur le principe de **CQS (Command-Query Separation)** introduit par Bertrand Meyer : *"une méthode doit soit modifier l'état du système (Command), soit retourner un résultat (Query), jamais les deux à la fois."*

CQRS pousse cette idée à l'échelle de l'architecture entière :

```
         ┌───────────────────────────────────────┐
         │              Application               │
         │                                       │
         │  ┌─────────────┐  ┌─────────────────┐ │
         │  │   Commands  │  │     Queries     │ │
         │  │  (Écritures)│  │   (Lectures)    │ │
         │  └──────┬──────┘  └────────┬────────┘ │
         │         │                  │          │
         │         ▼                  ▼          │
         │  ┌─────────────┐  ┌─────────────────┐ │
         │  │Write Model  │  │  Read Model     │ │
         │  │ (Domaine)   │  │  (Projection)   │ │
         │  └──────┬──────┘  └────────┬────────┘ │
         │         │                  │          │
         │         ▼                  ▼          │
         │  ┌─────────────┐  ┌─────────────────┐ │
         │  │  Write DB   │  │   Read DB / Vue │ │
         │  └─────────────┘  └─────────────────┘ │
         └───────────────────────────────────────┘
```

---

## Motivations

Les systèmes traditionnels utilisent un **modèle unique** pour lire et écrire les données (pattern CRUD classique). Cette approche atteint ses limites lorsque :

| Problème | Description |
|---|---|
| **Complexité métier** | Les règles de validation à l'écriture sont complexes mais les lectures sont simples et variées |
| **Asymétrie des charges** | La lecture est 10 à 100× plus fréquente que l'écriture dans la plupart des systèmes |
| **Contention de données** | Les transactions d'écriture bloquent les lectures sur les mêmes tables |
| **Modèle de données inadapté** | Un modèle normalisé (bon pour l'écriture) est peu performant pour les agrégations (lecture) |
| **Scalabilité** | Il est impossible de scaler indépendamment lecture et écriture |

CQRS résout ces problèmes en attribuant une **responsabilité unique** à chaque chemin.

---

## Principes : séparation lectures / écritures

### Commands (Écritures)

Une **Command** est une intention de **modifier l'état** du système.

- Elle est **nommée à l'impératif** : `CreerVenteCommand`, `AjusterStockCommand`, `ValiderCommandeFournisseurCommand`
- Elle **ne retourne pas de données métier** (au plus un identifiant ou un accusé de réception)
- Elle passe par les **agrégats du domaine** et applique les règles métier
- Elle peut émettre des **Domain Events** en cas de succès

```
Client ──► Command ──► Command Handler ──► Agrégat ──► Write DB
                                               │
                                               └──► Domain Event(s)
```

### Queries (Lectures)

Une **Query** est une demande de **lecture de données** sans effet de bord.

- Elle est **nommée comme une question** : `GetStockMedicamentQuery`, `ListVentesJourQuery`
- Elle **retourne un DTO optimisé** pour l'affichage (pas forcément une entité du domaine)
- Elle bypasse le modèle de domaine et lit directement depuis un **modèle de lecture optimisé**
- Elle peut utiliser des vues SQL, du cache, ou une base de données dédiée

```
Client ──► Query ──► Query Handler ──► Read DB / Cache ──► DTO ──► Client
```

---

## Implications sur les modèles et les données

### Write Model (Modèle d'écriture)

- Orienté **cohérence et intégrité** : utilise les agrégats DDD, les invariants métier
- Schéma **normalisé** (3NF), adapté aux transactions ACID
- Représente **l'état courant** de l'entité

### Read Model (Modèle de lecture)

- Orienté **performance et flexibilité** : des projections aplaties, dénormalisées
- Peut être une **vue SQL**, une table de cache, un index ElasticSearch, ou une base NoSQL
- Mis à jour via les Domain Events ou des mécanismes de synchronisation

### Synchronisation (Eventual Consistency)

Lorsque les deux modèles utilisent des stores différents, une **cohérence éventuelle** s'applique :

```
Write DB ──► Domain Event ──► Event Handler ──► Mise à jour Read DB
                  (via Service Bus, Azure Event Grid, etc.)
```

> ⚠️ La cohérence éventuelle signifie que les données lues peuvent être **légèrement en retard** sur les données écrites. Ce délai est généralement de l'ordre de quelques millisecondes à quelques secondes.

---

## Avantages et inconvénients

### ✅ Avantages

| Avantage | Détail |
|---|---|
| **Scalabilité indépendante** | On peut scaler les services de lecture séparément des services d'écriture |
| **Performances optimisées** | Chaque chemin utilise le modèle le plus adapté à son besoin |
| **Clarté du code** | Séparation nette des responsabilités, code plus lisible et maintenable |
| **Sécurité renforcée** | Contrôles d'accès distincts pour lecture et écriture |
| **Flexibilité des vues** | Plusieurs projections de lecture possibles pour le même état |
| **Facilite le DDD** | S'aligne naturellement avec les agrégats, Value Objects et Domain Events |

### ❌ Inconvénients

| Inconvénient | Détail |
|---|---|
| **Complexité accrue** | Plus de composants, plus de code à maintenir |
| **Cohérence éventuelle** | Les lectures peuvent être temporairement obsolètes après une écriture |
| **Synchronisation** | Mécanismes supplémentaires (messaging, projections) à gérer |
| **Courbe d'apprentissage** | Pattern plus difficile à appréhender pour les équipes débutantes |
| **Sur-ingénierie** | Inutile pour des applications CRUD simples à faible trafic |

---

## Quand utiliser CQRS ?

### ✅ CQRS est adapté quand…

```
✔ Le domaine est complexe (règles métier riches)
✔ Les charges de lecture et d'écriture sont très asymétriques
✔ Plusieurs représentations de la même donnée sont nécessaires (dashboards, API externes)
✔ L'application doit scaler de façon indépendante (lecture vs écriture)
✔ On utilise déjà DDD (Domain-Driven Design)
✔ On souhaite introduire Event Sourcing
```

### ❌ CQRS est déconseillé quand…

```
✘ L'application est un CRUD simple (gestion d'une liste de contacts, formulaire basique)
✘ L'équipe est petite et manque d'expérience avec les architectures distribuées
✘ Les exigences de cohérence stricte (strong consistency) sont impératives partout
✘ Le volume de données est faible et la performance n'est pas un enjeu
```

> 💡 **Conseil** : Il est possible d'appliquer CQRS **partiellement**, uniquement sur les sous-domaines les plus sollicités ou les plus complexes, en commençant par une séparation logique (même base de données, handlers séparés) avant d'évoluer vers une séparation physique.

---

## Lien avec Event Sourcing

CQRS et **Event Sourcing (ES)** sont deux patterns distincts mais **très complémentaires** — à tel point qu'on les cite souvent ensemble sous l'acronyme **CQRS/ES**.

### Event Sourcing en bref

Au lieu de stocker l'**état courant** d'une entité, on stocke la **séquence de tous les événements** qui ont conduit à cet état.

```
Vente #42 :
  [VenteCreee(date: 2024-01-15, pharmacien: "Konan")]
  [MedicamentAjoute(medicamentId: "M-001", qte: 2)]
  [PaiementRecu(montant: 5000, mode: "OrangeMoney")]
  [VenteValidee(total: 5000)]
         │
         └──► État courant reconstruit en rejouant les événements
```

### Comment CQRS et Event Sourcing se complètent

| Aspect | CQRS | Event Sourcing |
|---|---|---|
| **Rôle** | Sépare lecture et écriture | Stocke les événements plutôt que l'état |
| **Write Model** | Applique les Commands | Persiste les Domain Events dans un Event Store |
| **Read Model** | Projections optimisées | Projections reconstruites depuis les événements |
| **Synchronisation** | Via Domain Events | Via la relecture de l'Event Store |

```
Command ──► Agrégat ──► Domain Event ──► Event Store (Write)
                              │
                              └──► Projection ──► Read DB (Read)
```

### Bénéfices combinés CQRS + Event Sourcing

- **Audit trail complet** : l'historique de chaque modification est conservé (idéal pour les obligations légales pharmaceutiques)
- **Time travel** : possibilité de reconstruire l'état à n'importe quel instant passé
- **Débogage facilité** : on peut rejouer les événements pour comprendre un état anormal
- **Nouvelles projections** : créer un nouveau Read Model depuis les événements passés sans perte de données

---

## Exemple — API Pharmacie (Geocatis)

### Scénario : Enregistrer une vente

#### 1. Command (Écriture)

```csharp
// Définition de la Command
public record CreerVenteCommand(
    Guid PharmacienId,
    Guid? PatientId,
    IReadOnlyList<LigneVenteDto> Lignes,
    string ModePaiement
);

// Handler
public class CreerVenteCommandHandler
{
    public async Task<Guid> Handle(CreerVenteCommand command)
    {
        // 1. Charger les agrégats nécessaires
        var stock = await _stockRepository.GetByMedicamentIdsAsync(
            command.Lignes.Select(l => l.MedicamentId));

        // 2. Appliquer les règles métier (domaine)
        var vente = Vente.Creer(command.PharmacienId, command.PatientId);
        foreach (var ligne in command.Lignes)
        {
            vente.AjouterLigne(ligne.MedicamentId, ligne.Quantite, stock);
        }
        vente.ValiderPaiement(command.ModePaiement);

        // 3. Persister
        await _venteRepository.SaveAsync(vente);

        // 4. Publier les Domain Events (mise à jour du Read Model)
        await _eventBus.PublishAsync(vente.DomainEvents);

        return vente.Id;
    }
}
```

**Endpoint API :**
```
POST /api/ventes
Body: { "pharmacienId": "...", "lignes": [...], "modePaiement": "OrangeMoney" }
Response: 201 Created  { "venteId": "..." }
```

---

#### 2. Query (Lecture)

```csharp
// Définition de la Query
public record GetVentesJourQuery(DateOnly Date, Guid PharmacieId);

// DTO de réponse optimisé pour l'affichage
public record VenteResumeeDto(
    Guid Id,
    DateTime Date,
    string Pharmacien,
    decimal MontantTotal,
    string ModePaiement,
    int NombreLignes
);

// Handler — lit directement depuis le Read Model (sans passer par les agrégats)
public class GetVentesJourQueryHandler
{
    public async Task<IReadOnlyList<VenteResumeeDto>> Handle(GetVentesJourQuery query)
    {
        return await _readDb.VentesResumees
            .Where(v => v.Date == query.Date && v.PharmacieId == query.PharmacieId)
            .OrderByDescending(v => v.Date)
            .ToListAsync();
    }
}
```

**Endpoint API :**
```
GET /api/ventes?date=2024-01-15&pharmacieId=...
Response: 200 OK  [ { "id": "...", "montantTotal": 5000, ... }, ... ]
```

---

#### 3. Flux complet

```
Client (Blazor)
    │
    ├──► POST /api/ventes ──► CreerVenteCommandHandler
    │                                  │
    │                          Agrégat Vente
    │                                  │
    │                          Write DB (Azure SQL)
    │                                  │
    │                          Domain Event: VenteValidee
    │                                  │
    │                          Azure Service Bus
    │                                  │
    │                          VenteProjectionHandler
    │                                  │
    │                          Read DB (table VentesResumees)
    │
    └──► GET /api/ventes ──── GetVentesJourQueryHandler
                                       │
                               Read DB (requête optimisée)
                                       │
                               Liste VenteResumeeDto ──► Client
```

---

## Résumé

```
CQRS = séparer clairement les intentions
         ↓
Commands ──► modifient l'état ──► via les agrégats du domaine
         ↓
Queries  ──► lisent les données ──► via des projections optimisées
         ↓
Deux modèles indépendants, scalables séparément
         ↓
Couplage naturel avec DDD et Event Sourcing
         ↓
Adapté aux domaines complexes à fort trafic (ex : point de vente pharmacie)
```

> 📖 **Pour aller plus loin** :
> - [Martin Fowler — CQRS](https://martinfowler.com/bliki/CQRS.html)
> - [Microsoft — CQRS pattern](https://docs.microsoft.com/azure/architecture/patterns/cqrs)
> - [Greg Young — CQRS Documents](https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf)
