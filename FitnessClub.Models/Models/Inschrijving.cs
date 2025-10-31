using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models
{
    public class Inschrijving : BasisEntiteit
    {
        public int InschrijvingId { get; set; }

        [Required]
        [Display(Name = "Lid")]
        [ForeignKey("Lid")]
        public int LidId { get; set; } = Lid.Dummy?.Id ?? 0;

        [Required]
        [Display(Name = "Abonnement")]
        [ForeignKey("Abonnement")]
        public int AbonnementId { get; set; } = Abonnement.Dummy?.Id ?? 0;

        [Required]
        [Display(Name = "Startdatum")]
        public DateTime Startdatum { get; set; } = DateTime.Now;

        [Display(Name = "Einddatum")]
        public DateTime? Einddatum { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Actief";

        // Dummy object zoals leerkracht
        public static Inschrijving Dummy = null;

        // Navigation properties
        public virtual Lid Lid { get; set; }
        public virtual Abonnement Abonnement { get; set; }
        public virtual ICollection<Betaling> Betalingen { get; set; }

        public override string ToString()
        {
            return $"{Lid?.VolledigeNaam} - {Abonnement?.Naam}";
        }

        public static List<Inschrijving> SeedingData()
        {
            return new List<Inschrijving>
            {
                new Inschrijving { Verwijderd = DateTime.Now },
                new Inschrijving { LidId = 2, AbonnementId = 2, Startdatum = DateTime.Now.AddMonths(-2) },
                new Inschrijving { LidId = 3, AbonnementId = 3, Startdatum = DateTime.Now.AddMonths(-1) }
            };
        }
    }
}