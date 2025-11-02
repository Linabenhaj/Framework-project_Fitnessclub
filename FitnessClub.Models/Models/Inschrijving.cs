using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Inschrijving
    {
        public int Id { get; set; }

        [Required]
        public int AbonnementId { get; set; }

        [Required]
        public int LidId { get; set; }

        [Required]
        public DateTime StartDatum { get; set; }

        public DateTime? EindDatum { get; set; }

        // SOFT-DELETE
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }

        // Navigatie
        public virtual Abonnement Abonnement { get; set; }
        public virtual Lid Lid { get; set; }
        public virtual ICollection<Betaling> Betalingen { get; set; } = new List<Betaling>();
    }
}