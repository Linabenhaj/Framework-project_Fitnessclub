using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models
{
    public class Lid : BasisEntiteit
    {
        public int LidId { get; set; }

        [Required]
        [Display(Name = "Naam")]
        [StringLength(50)]
        public string Naam { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Voornaam")]
        [StringLength(50)]
        public string Voornaam { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Geboortedatum")]
        public DateTime Geboortedatum { get; set; }

        [Display(Name = "Contactgegevens")]
        [StringLength(100)]
        public string Contactgegevens { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Inschrijfdatum")]
        public DateTime Inschrijfdatum { get; set; } = DateTime.Now;

        // Dummy object zoals leerkracht
        public static Lid Dummy = null;

        public string VolledigeNaam => $"{Voornaam} {Naam}";

        public override string ToString()
        {
            return VolledigeNaam;
        }

        // Navigation property
        public virtual ICollection<Inschrijving> Inschrijvingen { get; set; }

        // Seeding data zoals leerkracht
        public static List<Lid> SeedingData()
        {
            return new List<Lid>
            {
                new Lid { Naam = "-", Voornaam = "-", Verwijderd = DateTime.Now },
                new Lid { Naam = "Janssens", Voornaam = "Jan", Geboortedatum = new DateTime(1990, 5, 15), Contactgegevens = "jan@email.com" },
                new Lid { Naam = "Peters", Voornaam = "Marie", Geboortedatum = new DateTime(1985, 8, 22), Contactgegevens = "marie@email.com" }
            };
        }
    }
}