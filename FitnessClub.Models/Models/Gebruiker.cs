using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models.Models
{
    public class Gebruiker : IdentityUser //Identity User klasse erft van IdentityUser
    {
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }
        public string? Adres { get; set; }
        public DateTime? Geboortedatum { get; set; }
        public string? Rol { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }
        public bool IsVerwijderd { get; set; } = false;

        //  zodat AccountController ook FirstName/LastName kan gebruiken
        [NotMapped]
        public string? FirstName
        {
            get => Voornaam;
            set => Voornaam = value;
        }

        [NotMapped]
        public string? LastName
        {
            get => Achternaam;
            set => Achternaam = value;
        }

        //  voor CreatedAt (gebruikt in AccountController)
        [NotMapped]
        public DateTime CreatedAt
        {
            get => AangemaaktOp;
            set => AangemaaktOp = value;
        }

        // Relatie naar Abonnement
        public int? AbonnementId { get; set; }
        public virtual Abonnement? Abonnement { get; set; }

        // Collectie van Inschrijvingen: belangrijk voor relatie naar inschrijvingen
        public virtual ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();

        [NotMapped]
        public int? Leeftijd //enige property die not mapped is, omdat het niet wordt opgeslagen in database enkel berekend
        {
            get
            {
                if (!Geboortedatum.HasValue)
                    return null;

                var today = DateTime.Today;
                var age = today.Year - Geboortedatum.Value.Year;

                if (Geboortedatum.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
    }
}