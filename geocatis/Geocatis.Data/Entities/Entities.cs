using System;
using System.Collections.Generic;

namespace Geocatis.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // admin, régie, annonceur
    }

    public class Panneau
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Localisation { get; set; }
        public string Type { get; set; }
        public bool Disponible { get; set; }
    }

    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid PanneauId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Statut { get; set; }
    }
}
