# Modèle de Domaine - Application de Gestion de Pharmacie

## Introduction

Ce document décrit le modèle de domaine métier de l'application de gestion de pharmacie pour la Côte d'Ivoire. Il définit les entités principales, leurs relations et les règles métier associées.

---

## Diagramme de Domaine

```
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│   Utilisateur   │      │   Pharmacie     │      │   Fournisseur   │
├─────────────────┤      ├─────────────────┤      ├─────────────────┤
│ Id              │      │ Id              │      │ Id              │
│ Nom             │      │ Nom             │      │ RaisonSociale   │
│ Email           │◄────►│ Adresse         │      │ Email           │
│ Role            │      │ Téléphone       │      │ Téléphone       │
│ PharmacieId     │      │ NumAutorisation │      │ Adresse         │
└────────┬────────┘      └────────┬────────┘      └────────┬────────┘
         │                        │                        │
         │                        │                        │
         ▼                        ▼                        ▼
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│    Vente        │      │   Médicament    │◄────►│  Commande       │
├─────────────────┤      ├─────────────────┤      ├─────────────────┤
│ Id              │      │ Id              │      │ Id              │
│ Date            │◄────►│ CodeCIP         │      │ DateCommande    │
│ PatientId       │      │ Nom             │      │ FournisseurId   │
│ UtilisateurId   │      │ DCI             │      │ Statut          │
│ MontantTotal    │      │ Forme           │      │ MontantTotal    │
│ ModePaiement    │      │ Dosage          │      └────────┬────────┘
└────────┬────────┘      │ PrixVente       │               │
         │               │ StockActuel     │               │
         │               │ SeuilAlerte     │               │
         │               └────────┬────────┘               │
         │                        │                        │
         ▼                        ▼                        ▼
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│  LigneVente     │      │  Stock          │      │ LigneCommande   │
├─────────────────┤      ├─────────────────┤      ├─────────────────┤
│ Id              │      │ Id              │      │ Id              │
│ VenteId         │◄────►│ MédicamentId    │◄────►│ CommandeId      │
│ MédicamentId    │      │ Quantité        │      │ MédicamentId    │
│ Quantité        │      │ DatePeremption  │      │ Quantité        │
│ PrixUnitaire    │      │ NumLot          │      │ PrixUnitaire    │
│ Remise          │      │ FournisseurId   │      └─────────────────┘
└─────────────────┘      └─────────────────┘

┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│    Patient      │      │  Prescription   │      │  Inventaire     │
├─────────────────┤      ├─────────────────┤      ├─────────────────┤
│ Id              │◄────►│ Id              │      │ Id              │
│ Nom             │      │ PatientId       │      │ Date            │
│ Prénom          │      │ MédecinId       │      │ UtilisateurId   │
│ DateNaissance   │      │ Date            │      │ Statut          │
│ Téléphone       │      │ Médicaments[]   │      │ Commentaire     │
│ NumCMU          │      │ Posologie       │      └────────┬────────┘
│ Allergies       │      └─────────────────┘               │
└─────────────────┘                                        ▼
                                                  ┌─────────────────┐
                                                  │ LigneInventaire │
                                                  ├─────────────────┤
                                                  │ Id              │
                                                  │ InventaireId    │
                                                  │ MédicamentId    │
                                                  │ QteThéorique    │
                                                  │ QteRéelle       │
                                                  │ Ecart           │
                                                  └─────────────────┘
```

---

## Entités principales

### 1. Pharmacie

Représente l'établissement pharmaceutique.

```csharp
public class Pharmacie
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string RaisonSociale { get; set; }
    public string Adresse { get; set; }
    public string Ville { get; set; }
    public string CodePostal { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string NumeroAutorisation { get; set; } // Autorisation de l'Ordre
    public string NumeroRCCM { get; set; } // Registre du Commerce
    public string NumeroCC { get; set; } // Compte Contribuable
    public DateTime DateCreation { get; set; }
    public bool EstActive { get; set; }
    
    // Relations
    public ICollection<Utilisateur> Utilisateurs { get; set; }
    public ICollection<Vente> Ventes { get; set; }
    public ICollection<Stock> Stocks { get; set; }
}
```

### 2. Utilisateur

Représente les utilisateurs du système (pharmaciens, préparateurs, etc.).

```csharp
public class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public RoleUtilisateur Role { get; set; }
    public int PharmacieId { get; set; }
    public string NumeroOrdre { get; set; } // Pour les pharmaciens
    public bool EstActif { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DerniereConnexion { get; set; }
    
    // Relations
    public Pharmacie Pharmacie { get; set; }
    public ICollection<Vente> Ventes { get; set; }
}

public enum RoleUtilisateur
{
    SuperAdmin,
    GerantPharmacie,
    Pharmacien,
    Preparateur,
    Caissier,
    Auditeur
}
```

### 3. Médicament

Représente un produit pharmaceutique.

```csharp
public class Medicament
{
    public int Id { get; set; }
    public string CodeCIP { get; set; } // Code Identifiant Présentation
    public string CodeEAN { get; set; } // Code-barres
    public string Nom { get; set; }
    public string DCI { get; set; } // Dénomination Commune Internationale
    public string Forme { get; set; } // Comprimé, Sirop, Injectable, etc.
    public string Dosage { get; set; }
    public string Fabricant { get; set; }
    public string Conditionnement { get; set; }
    
    // Prix
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public decimal TauxTVA { get; set; } // 18% en CIV
    public decimal? TauxRemise { get; set; }
    
    // Stock
    public int StockActuel { get; set; }
    public int SeuilAlerte { get; set; }
    public int StockMinimum { get; set; }
    public int StockMaximum { get; set; }
    
    // Classification
    public ClasseMedicament Classe { get; set; }
    public bool EstStupefiant { get; set; }
    public bool SurOrdonnance { get; set; }
    public bool EstGenerique { get; set; }
    public bool EstActif { get; set; }
    
    // Métadonnées
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string Indications { get; set; }
    public string ContreIndications { get; set; }
    public string Posologie { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    
    // Relations
    public ICollection<Stock> Stocks { get; set; }
    public ICollection<LigneVente> LignesVente { get; set; }
    public ICollection<LigneCommande> LignesCommande { get; set; }
}

public enum ClasseMedicament
{
    Antalgique,
    Antibiotique,
    Antiinflammatoire,
    Antihypertenseur,
    Antidiabetique,
    Antipaludeen,
    Vitamines,
    Autres
}
```

### 4. Stock

Représente un lot de médicament en stock.

```csharp
public class Stock
{
    public int Id { get; set; }
    public int MedicamentId { get; set; }
    public int PharmacieId { get; set; }
    public int? FournisseurId { get; set; }
    
    public string NumeroLot { get; set; }
    public int Quantite { get; set; }
    public DateTime DatePeremption { get; set; }
    public DateTime DateReception { get; set; }
    public decimal PrixAchatUnitaire { get; set; }
    
    public bool EstPerime => DateTime.Now > DatePeremption;
    public bool ProcheDeLaPeremption => DatePeremption.AddDays(-90) <= DateTime.Now;
    
    // Relations
    public Medicament Medicament { get; set; }
    public Pharmacie Pharmacie { get; set; }
    public Fournisseur Fournisseur { get; set; }
}
```

### 5. Patient

Représente un patient/client.

```csharp
public class Patient
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public DateTime DateNaissance { get; set; }
    public string Sexe { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    
    // Assurance
    public string NumeroCMU { get; set; } // Couverture Maladie Universelle
    public string NumeroAssurance { get; set; }
    public string NomAssurance { get; set; }
    public decimal? TauxRemboursement { get; set; }
    
    // Santé
    public string Allergies { get; set; }
    public string PathologiesChroniques { get; set; }
    public string TraitementsEnCours { get; set; }
    
    public DateTime DateCreation { get; set; }
    public DateTime? DernierAchat { get; set; }
    
    // Relations
    public ICollection<Vente> Ventes { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; }
}
```

### 6. Vente

Représente une transaction de vente.

```csharp
public class Vente
{
    public int Id { get; set; }
    public string NumeroVente { get; set; } // Auto-généré
    public DateTime DateVente { get; set; }
    
    public int? PatientId { get; set; }
    public int UtilisateurId { get; set; }
    public int PharmacieId { get; set; }
    public int? PrescriptionId { get; set; }
    
    public decimal MontantHT { get; set; }
    public decimal MontantTVA { get; set; }
    public decimal MontantTTC { get; set; }
    public decimal MontantRemise { get; set; }
    public decimal MontantPaye { get; set; }
    public decimal MontantRendu { get; set; }
    
    public ModePaiement ModePaiement { get; set; }
    public string ReferencePaiement { get; set; } // Pour paiements mobiles
    
    public StatutVente Statut { get; set; }
    public string Commentaire { get; set; }
    
    // Relations
    public Patient Patient { get; set; }
    public Utilisateur Utilisateur { get; set; }
    public Pharmacie Pharmacie { get; set; }
    public Prescription Prescription { get; set; }
    public ICollection<LigneVente> Lignes { get; set; }
}

public enum ModePaiement
{
    Especes,
    CarteBancaire,
    OrangeMoney,
    MTNMobileMoney,
    MoovMoney,
    Wave,
    Cheque,
    Virement,
    Assurance
}

public enum StatutVente
{
    EnCours,
    Validee,
    Annulee,
    Remboursee
}
```

### 7. LigneVente

Représente une ligne de vente (un médicament dans une vente).

```csharp
public class LigneVente
{
    public int Id { get; set; }
    public int VenteId { get; set; }
    public int MedicamentId { get; set; }
    public int? StockId { get; set; } // Lot utilisé
    
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal TauxRemise { get; set; }
    public decimal TauxTVA { get; set; }
    
    public decimal MontantHT => (PrixUnitaire * Quantite) * (1 - TauxRemise / 100);
    public decimal MontantTVA => MontantHT * (TauxTVA / 100);
    public decimal MontantTTC => MontantHT + MontantTVA;
    
    // Relations
    public Vente Vente { get; set; }
    public Medicament Medicament { get; set; }
    public Stock Stock { get; set; }
}
```

### 8. Fournisseur

Représente un fournisseur de médicaments.

```csharp
public class Fournisseur
{
    public int Id { get; set; }
    public string RaisonSociale { get; set; }
    public string Adresse { get; set; }
    public string Ville { get; set; }
    public string Pays { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string NumeroRCCM { get; set; }
    public string NumeroCC { get; set; }
    
    public int DelaiLivraison { get; set; } // En jours
    public decimal MontantMinimumCommande { get; set; }
    public bool EstActif { get; set; }
    
    public DateTime DateCreation { get; set; }
    
    // Relations
    public ICollection<Commande> Commandes { get; set; }
    public ICollection<Stock> Stocks { get; set; }
}
```

### 9. Commande

Représente une commande fournisseur.

```csharp
public class Commande
{
    public int Id { get; set; }
    public string NumeroCommande { get; set; }
    public DateTime DateCommande { get; set; }
    public DateTime? DateLivraisonPrevue { get; set; }
    public DateTime? DateLivraisonReelle { get; set; }
    
    public int FournisseurId { get; set; }
    public int UtilisateurId { get; set; }
    public int PharmacieId { get; set; }
    
    public decimal MontantTotal { get; set; }
    public StatutCommande Statut { get; set; }
    public string Commentaire { get; set; }
    
    // Relations
    public Fournisseur Fournisseur { get; set; }
    public Utilisateur Utilisateur { get; set; }
    public Pharmacie Pharmacie { get; set; }
    public ICollection<LigneCommande> Lignes { get; set; }
}

public enum StatutCommande
{
    Brouillon,
    Envoyee,
    Confirmee,
    EnLivraison,
    Livree,
    Annulee
}
```

### 10. LigneCommande

```csharp
public class LigneCommande
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public int MedicamentId { get; set; }
    
    public int QuantiteCommandee { get; set; }
    public int? QuantiteRecue { get; set; }
    public decimal PrixUnitaire { get; set; }
    
    public decimal MontantTotal => PrixUnitaire * QuantiteCommandee;
    
    // Relations
    public Commande Commande { get; set; }
    public Medicament Medicament { get; set; }
}
```

### 11. Prescription

Représente une ordonnance médicale.

```csharp
public class Prescription
{
    public int Id { get; set; }
    public string NumeroPrescription { get; set; }
    public DateTime DatePrescription { get; set; }
    public DateTime DateExpiration { get; set; }
    
    public int PatientId { get; set; }
    public string MedecinNom { get; set; }
    public string MedecinNumeroOrdre { get; set; }
    public string MedecinTelephone { get; set; }
    
    public string Diagnostic { get; set; }
    public string Posologie { get; set; }
    public int DureeTraitement { get; set; } // En jours
    
    public string DocumentUrl { get; set; } // Scan de l'ordonnance
    public bool EstValidee { get; set; }
    
    // Relations
    public Patient Patient { get; set; }
    public ICollection<Vente> Ventes { get; set; }
}
```

### 12. Inventaire

Représente un inventaire physique du stock.

```csharp
public class Inventaire
{
    public int Id { get; set; }
    public string NumeroInventaire { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    
    public int UtilisateurId { get; set; }
    public int PharmacieId { get; set; }
    
    public StatutInventaire Statut { get; set; }
    public string Commentaire { get; set; }
    
    public decimal EcartValeurTotal { get; set; }
    
    // Relations
    public Utilisateur Utilisateur { get; set; }
    public Pharmacie Pharmacie { get; set; }
    public ICollection<LigneInventaire> Lignes { get; set; }
}

public enum StatutInventaire
{
    EnCours,
    Valide,
    Annule
}
```

### 13. LigneInventaire

```csharp
public class LigneInventaire
{
    public int Id { get; set; }
    public int InventaireId { get; set; }
    public int MedicamentId { get; set; }
    
    public int QuantiteTheorique { get; set; }
    public int QuantiteReelle { get; set; }
    public int Ecart => QuantiteReelle - QuantiteTheorique;
    
    public decimal PrixUnitaire { get; set; }
    public decimal EcartValeur => Ecart * PrixUnitaire;
    
    public string Commentaire { get; set; }
    
    // Relations
    public Inventaire Inventaire { get; set; }
    public Medicament Medicament { get; set; }
}
```

---

## Règles métier

### Gestion des stocks

1. **Mise à jour automatique** : Le stock est décrémenté automatiquement lors d'une vente validée
2. **Méthode FEFO** : First Expired, First Out - les lots avec la date de péremption la plus proche sont vendus en premier
3. **Alerte stock bas** : Notification quand stock < seuil d'alerte
4. **Alerte péremption** : Notification 90 jours avant la date de péremption
5. **Blocage vente** : Impossible de vendre un médicament périmé
6. **Quarantaine** : Les médicaments proches de la péremption (< 30 jours) nécessitent une confirmation

### Ventes

1. **Prescription obligatoire** : Certains médicaments nécessitent une ordonnance valide
2. **Stupéfiants** : Traçabilité renforcée avec conservation de l'ordonnance originale
3. **Limite de quantité** : Certains médicaments ont une quantité maximale par vente
4. **Vérification interactions** : Alerte en cas d'interactions médicamenteuses connues
5. **TVA** : Application automatique du taux de 18%
6. **Remise** : Possibilité de remise par ligne ou globale (avec autorisation)

### Prescriptions

1. **Validité** : Une ordonnance est valable 3 mois (sauf stupéfiants : 7 jours)
2. **Renouvelable** : Certaines prescriptions peuvent être renouvelées
3. **Validation pharmacien** : Un pharmacien doit valider chaque prescription
4. **Conservation** : Les ordonnances doivent être conservées 3 ans

### Commandes fournisseurs

1. **Calcul automatique** : Suggestion de commande basée sur la rotation et le stock
2. **Quantité minimum** : Respect de la quantité minimum de commande du fournisseur
3. **Réception partielle** : Possibilité de recevoir une commande en plusieurs fois
4. **Contrôle qualité** : Vérification des lots et dates de péremption à la réception

### Inventaire

1. **Périodicité** : Inventaire complet au moins annuel
2. **Validation** : L'inventaire doit être validé par un responsable
3. **Ajustement automatique** : Le stock système est ajusté après validation
4. **Audit trail** : Toutes les modifications sont tracées

### Sécurité et conformité

1. **Traçabilité totale** : Chaque action est loguée avec utilisateur, date, heure
2. **Conservation légale** : Les données de vente sont conservées 7 ans minimum
3. **Données sensibles** : Les informations patient sont chiffrées
4. **RGPD** : Consentement obligatoire pour la collecte de données
5. **Droit à l'oubli** : Anonymisation après demande (sauf obligation légale)

---

## Points d'extension

### Fonctionnalités futures

1. **Programme de fidélité** : Points de fidélité et récompenses
2. **Téléconsultation** : Intégration avec plateformes de télémédecine
3. **E-commerce** : Vente en ligne avec livraison
4. **IA prédictive** : Prévision des besoins en stock
5. **Reconnaissance d'image** : OCR pour ordonnances scannées
6. **Multi-sites** : Gestion de plusieurs pharmacies
7. **B2B** : Vente aux structures de santé (hôpitaux, cliniques)
8. **Analyse de données** : Business Intelligence et reporting avancé

---

## Conclusion

Ce modèle de domaine fournit une base solide pour une application complète de gestion de pharmacie, adaptée aux besoins spécifiques du marché ivoirien tout en respectant les standards internationaux et les bonnes pratiques du secteur pharmaceutique.
