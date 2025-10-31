using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models
{
    public abstract class BasisEntiteit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Aangemaakt op")]
        public DateTime AangemaaktOp { get; set; } = DateTime.Now;

        [Display(Name = "Gewijzigd op")]
        public DateTime? GewijzigdOp { get; set; }

        [Required]
        [Display(Name = "Verwijderd")]
        public DateTime Verwijderd { get; set; } = DateTime.MaxValue; // SOFT-DELETE zoals leerkracht
    }
}