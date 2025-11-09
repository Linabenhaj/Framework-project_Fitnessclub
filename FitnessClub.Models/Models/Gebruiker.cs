using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models
{
    public class Gebruiker : IdentityUser
    {
        [NotMapped]
        public string DisplayNaam { get; set; }
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public DateTime Geboortedatum { get; set; } = DateTime.Now.AddYears(-20);
        public string Telefoon { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        // Abonnement relatie
        public int? AbonnementId { get; set; }
        public Abonnement Abonnement { get; set; }

        public ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();



        // Soft delete properties
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }
    }
}