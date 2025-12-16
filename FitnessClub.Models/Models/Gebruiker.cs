using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema; // Voeg deze using toe

namespace FitnessClub.Models.Models
{
    public class Gebruiker : IdentityUser
    {
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }
        public string? Adres { get; set; }
        public DateTime? Geboortedatum { get; set; }
        public string? Rol { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }
        public bool IsVerwijderd { get; set; } = false;

        // Relatie naar Abonnement
        public int? AbonnementId { get; set; }
        public virtual Abonnement? Abonnement { get; set; }

        // Collectie van Inschrijvingen
        public virtual ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();

        // Berekenende property voor Leeftijd
        [NotMapped] // Zorgt dat deze NIET in de database komt
        public int? Leeftijd
        {
            get
            {
                if (!Geboortedatum.HasValue)
                    return null;

                var today = DateTime.Today;
                var age = today.Year - Geboortedatum.Value.Year;

                // Aanpassing als verjaardag dit jaar nog niet geweest is
                if (Geboortedatum.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }

      
    }
}