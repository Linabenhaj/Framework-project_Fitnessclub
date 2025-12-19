using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models.Models
{
    public class Abonnement : BasisEntiteit
    {
        [Required]
        [StringLength(100)]
        public string Naam { get; set; } = string.Empty;

        [StringLength(500)]
        public string Beschrijving { get; set; } = string.Empty;

        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Prijs { get; set; }

        [Range(1, 36)]
        public int DuurInMaanden { get; set; } = 1;

        public bool IsActief { get; set; } = true;

        public virtual ICollection<Gebruiker> Gebruikers { get; set; } = new List<Gebruiker>();
    }
}