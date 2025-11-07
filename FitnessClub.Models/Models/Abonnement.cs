using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models
{
    public class Abonnement : BasisEntiteit
    {
        [Required]
        public string Naam { get; set; }

        [Range(0, 999.99)]
        public decimal Prijs { get; set; }

        public string Omschrijving { get; set; }

        // Looptijd in maanden
        public int LooptijdMaanden { get; set; } = 1;

        // Relatie met Gebruikers
        public ICollection<Gebruiker> Gebruikers { get; set; } = new List<Gebruiker>();
    }
}