# MVP Geocatis – Spécification et Plan d’Actions

## Objectif
Permettre aux régies et annonceurs de visualiser les panneaux sur une carte, de réserver un panneau, et de gérer l’authentification, le tout dans une web app Azure (Blazor).

---

## Fonctionnalités du MVP

1. **Authentification et gestion des rôles**
   - Inscription/connexion (admin, régie, annonceur)
   - Sécurité des accès

2. **Visualisation géolocalisée des panneaux**
   - Carte interactive (Azure Maps)
   - Liste des panneaux avec détails (emplacement, type, disponibilité)

3. **Réservation de panneaux**
   - Réservation simple depuis la carte ou la liste
   - Gestion des disponibilités (calendrier basique)

4. **Notifications basiques**
   - Notification in-app ou email lors d’une réservation

---

## Plan d’actions détaillé

### 1. Initialisation du projet
- Créer la solution .NET 9
- Ajouter un projet Blazor Server (meilleur choix pour MVP rapide, sécurité et intégration Azure)
- Ajouter un projet API REST (ASP.NET Core)
- Ajouter un projet Data (accès base de données)

### 2. Authentification et gestion des rôles
- Intégrer Azure AD B2C ou IdentityServer pour l’authentification
- Créer les entités User et Role
- Implémenter la gestion des rôles (admin, régie, annonceur)

### 3. Visualisation des panneaux
- Créer l’entité Panneau (id, nom, localisation, type, disponibilité)
- Intégrer Azure Maps dans Blazor
- Afficher les panneaux sur la carte et dans une liste
- Ajouter des filtres de recherche

### 4. Réservation de panneaux
- Créer l’entité Réservation (id, panneauId, userId, dateDébut, dateFin, statut)
- Permettre la réservation depuis la carte ou la liste
- Afficher les disponibilités (calendrier simple)
- Gérer les conflits de réservation

### 5. Notifications
- Intégrer Azure Notification Hubs ou envoi d’email (SendGrid)
- Envoyer une notification lors d’une nouvelle réservation ou annulation

### 6. Tests et déploiement
- Écrire des tests unitaires pour les principales fonctionnalités
- Déployer sur Azure App Service

---

## Structure de projet recommandée

```
geocatis/
│
├── README.md
├── MVP_PLAN.md
├── src/
│   ├── Geocatis.Web/           # Projet Blazor Server
│   ├── Geocatis.API/           # API REST .NET
│   └── Geocatis.Data/          # Accès base de données
├── docs/
│   └── architecture.md
└── .github/
    └── workflows/
```

---

## Choix technique Blazor
Pour le MVP, **Blazor Server** est recommandé : rapidité de développement, sécurité, intégration Azure facilitée, gestion de l’état côté serveur.

---

## Prochaines étapes
1. Générer le squelette de la solution .NET 9 avec les projets nécessaires.
2. Implémenter les entités et les premières pages Blazor.
3. Intégrer l’authentification et la carte.
4. Développer la réservation et les notifications.
5. Tester et déployer.
