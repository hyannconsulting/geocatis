# 📋 Backlog des Issues Principales pour le projet Geocatis

Ce fichier regroupe les issues principales à créer pour le projet **geocatis**. Chaque issue suit le template MCP et inclut description, critères d’acceptation, références, dépendances, estimation d’effort et labels recommandés.

---

## 1. Visualisation géolocalisée des panneaux

### Description
Mettre en place une carte interactive affichant tous les panneaux publicitaires disponibles, avec filtres par zone, type, audience, etc.

### Critères d’acceptation
- [ ] Les utilisateurs peuvent visualiser les panneaux sur une carte.
- [ ] Les filtres sont fonctionnels et dynamiques.
- [ ] Les informations détaillées d’un panneau sont accessibles via la carte.

### Référence d’implémentation
- Utilisation d’Azure Maps ou équivalent.
- Inspiration : [Google Maps Marker Clustering](https://developers.google.com/maps/documentation/javascript/marker-clustering)

### Dépendances
- **Bloque** : Réservation de panneaux
- **Bloqué par** : Authentification

### Estimation d’effort
Large

**Labels** : enhancement, MCP, frontend, priorité-haute

---

## 2. Réservation de panneaux et calendrier partagé

### Description
Permettre aux annonceurs de réserver un panneau pour une période donnée, avec gestion des disponibilités en temps réel.

### Critères d’acceptation
- [ ] Un utilisateur peut réserver un panneau depuis la carte ou la liste.
- [ ] Le calendrier affiche les disponibilités.
- [ ] Les conflits de réservation sont gérés.

### Référence d’implémentation
- Utilisation d’un composant calendrier (ex : FullCalendar)
- Documentation Azure SQL pour la gestion des disponibilités

### Dépendances
- **Bloqué par** : Visualisation géolocalisée des panneaux, Authentification

### Estimation d’effort
Large

**Labels** : enhancement, MCP, frontend, backend, priorité-haute

---

## 3. Recommandations IA personnalisées

### Description
Proposer automatiquement des panneaux pertinents à l’annonceur selon sa cible, son budget et la localisation, via Azure OpenAI.

### Critères d’acceptation
- [ ] Les recommandations sont affichées lors de la recherche ou de la réservation.
- [ ] L’utilisateur peut filtrer ou ajuster les recommandations.

### Référence d’implémentation
- Azure OpenAI Service
- Exemples de recommandations dans le secteur publicitaire

### Dépendances
- **Bloqué par** : Réservation de panneaux, Visualisation géolocalisée

### Estimation d’effort
Medium

**Labels** : enhancement, MCP, IA, priorité-moyenne

---

## 4. Notifications multicanal

### Description
Envoyer des notifications (email, SMS, in-app) lors des changements de statut des réservations ou autres événements clés.

### Critères d’acceptation
- [ ] L’utilisateur reçoit une notification pour chaque événement important.
- [ ] Les préférences de notification sont configurables.

### Référence d’implémentation
- Azure Notification Hubs
- Documentation SendGrid/Azure Communication Services

### Dépendances
- **Bloqué par** : Réservation de panneaux

### Estimation d’effort
Medium

**Labels** : enhancement, MCP, notifications, priorité-moyenne

---

## 5. Gestion avancée des droits et rôles

### Description
Mettre en place un système de gestion des rôles (admin, régie, annonceur) avec permissions fines.

### Critères d’acceptation
- [ ] Les accès sont restreints selon le rôle.
- [ ] L’admin peut gérer les utilisateurs et leurs droits.

### Référence d’implémentation
- Azure AD Auth
- Exemples de RBAC (Role-Based Access Control)

### Dépendances
- **Bloqué par** : Authentification

### Estimation d’effort
Medium

**Labels** : enhancement, MCP, sécurité, backend, priorité-haute

---

## 6. Tableau de bord et reporting

### Description
Créer un tableau de bord pour visualiser les statistiques d’occupation, de performance et d’utilisation.

### Critères d’acceptation
- [ ] Les statistiques sont accessibles et exportables.
- [ ] Les données sont filtrables par période, type, etc.

### Référence d’implémentation
- Power BI Embedded, Azure SQL Analytics

### Dépendances
- **Bloqué par** : Réservation de panneaux

### Estimation d’effort
Medium

**Labels** : enhancement, MCP, reporting, frontend, priorité-moyenne

---

## 7. Authentification et gestion des utilisateurs

### Description
Mettre en place l’authentification sécurisée et la gestion des utilisateurs.

### Critères d’acceptation
- [ ] Les utilisateurs peuvent s’inscrire, se connecter et gérer leur profil.
- [ ] Les accès sont sécurisés.

### Référence d’implémentation
- Azure AD B2C, IdentityServer

### Dépendances
- **Bloque** : Toutes les autres fonctionnalités

### Estimation d’effort
Medium

**Labels** : enhancement, MCP, sécurité, backend, priorité-haute
