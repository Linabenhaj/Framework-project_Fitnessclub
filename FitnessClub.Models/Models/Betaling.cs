using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models
{
    public class Betaling : BasisEntiteit
    {
        // Unieke identifier voor de betaling
        public int BetalingId { get; set; }

        // Foreign key naar de inschrijving
        [Required]
        [Display(Name = "Inschrijving")]
        [ForeignKey("Inschrijving")]
        public int InschrijvingId { get; set; } = Inschrijving.Dummy?.Id ?? 0;

        [Required]
        [Display(Name = "Bedrag")]
        [Range(0.01, 10000)]
        public decimal Bedrag { get; set; }

        [Required]
        [Display(Name = "Betaaldatum")]
        public DateTime Betaaldatum { get; set; }

        [Required]
        [Display(Name = "Betaalmethode")]
        public string Betaalmethode { get; set; } = "Overschrijving";

        // Status van de betaling
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Betaald";

        // Dummy object
        public static Betaling Dummy = null;

        // Navigatie naar de inschrijving
        public virtual Inschrijving Inschrijving { get; set; }

        // Toont betaling info als string
        public override string ToString()
        {
            return $"{Bedrag:C} - {Betaaldatum:dd/MM/yyyy}";
        }

        // Test data 
        public static List<Betaling> SeedingData()
        {
            return new List<Betaling>
            {
                new Betaling { Verwijderd = DateTime.Now },
                new Betaling { InschrijvingId = 2, Bedrag = 25.00m, Betaaldatum = DateTime.Now.AddMonths(-2) },
                new Betaling { InschrijvingId = 2, Bedrag = 25.00m, Betaaldatum = DateTime.Now.AddMonths(-1) },
                new Betaling { InschrijvingId = 3, Bedrag = 45.00m, Betaaldatum = DateTime.Now.AddMonths(-1) }
            };
        }
    }
}
