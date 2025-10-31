using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models
{
    public class Abonnement : BasisEntiteit
    {
        public int AbonnementId { get; set; }

        [Required]
        [Display(Name = "Naam")]
        [StringLength(50)]
        public string Naam { get; set; } = string.Empty;

        [Display(Name = "Omschrijving")]
        [StringLength(200)]
        public string Omschrijving { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Maandelijkse prijs")]
        [Range(0, 1000)]
        public decimal MaandelijksePrijs { get; set; }

        [Required]
        [Display(Name = "Looptijd (maanden)")]
        [Range(1, 36)]
        public int LooptijdMaanden { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Actief";

        // Dummy object zoals leerkracht
        public static Abonnement Dummy = null;

        // Navigation property
        public virtual ICollection<Inschrijving> Inschrijvingen { get; set; }

        public override string ToString()
        {
            return $"{Naam} - €{MaandelijksePrijs}/maand";
        }

        public static List<Abonnement> SeedingData()
        {
            return new List<Abonnement>
            {
                new Abonnement { Naam = "-", Omschrijving = "-", Verwijderd = DateTime.Now },
                new Abonnement { Naam = "Basic", Omschrijving = "Basis abonnement", MaandelijksePrijs = 25.00m, LooptijdMaanden = 1 },
                new Abonnement { Naam = "Premium", Omschrijving = "Premium abonnement met extra features", MaandelijksePrijs = 45.00m, LooptijdMaanden = 1 },
                new Abonnement { Naam = "Jaarabonnement", Omschrijving = "Voordelig jaarabonnement", MaandelijksePrijs = 35.00m, LooptijdMaanden = 12 }
            };
        }
    }
}