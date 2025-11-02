using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Betaling
    {
        public int Id { get; set; }

        [Required]
        public int LidId { get; set; }

        [Required]
        public int InschrijvingId { get; set; }

        [Range(0, 999.99)]
        public decimal Bedrag { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        public bool IsBetaald { get; set; } = true;

        // SOFT-DELETE 
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }

        // Navigatie
        public virtual Lid Lid { get; set; }
        public virtual Inschrijving Inschrijving { get; set; }
    }
}