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

        public int LooptijdMaanden { get; set; }

        // SOFT-DELETE 
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }

        // Navigatie
        public virtual ICollection<Lid> Leden { get; set; } = new List<Lid>();
    }
}