using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FitnessClub.Models
{
    public class Gebruiker : IdentityUser
    {
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public DateTime Geboortedatum { get; set; } = DateTime.Now;
        public string Telefoon { get; set; } = string.Empty;

        // Abonnement relatie
        public int? AbonnementId { get; set; }
        public Abonnement Abonnement { get; set; }

        public ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();

        // Soft delete
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }
    }
}