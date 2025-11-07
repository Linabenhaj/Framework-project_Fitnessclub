using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Abonnement
    {
        public int Id { get; set; }

        [Required]
        public string Naam { get; set; }

        [Range(0, 999.99)]
        public decimal Prijs { get; set; }
        public string Omschrijving { get; set; }

        // Navigatie
        public virtual ICollection<Lid> Leden { get; set; } = new List<Lid>();
    }
}