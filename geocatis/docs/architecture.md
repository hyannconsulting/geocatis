# Architecture technique Geocatis

## Couches principales

- **Geocatis.Web** : Application Blazor Server (UI, gestion de l’état, intégration Azure Maps, notifications)
- **Geocatis.API** : API REST .NET Core (exposition des données, logique métier, sécurisation des accès)
- **Geocatis.Data** : Accès base de données, entités, repository

## Technologies

- .NET 9, C#
- Blazor Server (UI)
- Azure Maps (cartographie)
- Azure AD B2C (authentification et gestion des rôles)
- Entity Framework Core (accès base de données)
- Azure Notification Hubs ou SendGrid (notifications)

## Flux principaux

1. **Authentification** :
   - Utilisateur se connecte via Azure AD B2C (SSO, gestion des rôles)
   - Jeton d’accès transmis à l’API pour sécuriser les appels

2. **Visualisation des panneaux** :
   - L’utilisateur consulte la carte interactive (Azure Maps)
   - Les panneaux sont récupérés via l’API REST

3. **Réservation** :
   - L’utilisateur réserve un panneau depuis la carte ou la liste
   - L’API vérifie la disponibilité, crée la réservation et notifie l’utilisateur

4. **Notifications** :
   - Notification envoyée lors d’une réservation ou annulation (in-app ou email)

## Sécurité

- Authentification centralisée Azure AD B2C
- Gestion des rôles (admin, régie, annonceur)
- Sécurisation des endpoints API

## Schéma simplifié

```
[Utilisateur] ⇄ [Blazor Web] ⇄ [API REST sécurisée] ⇄ [Data/DB]
           ⬑ Azure AD B2C (auth)
           ⬐ Azure Maps, Notification Hubs
```
