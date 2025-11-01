using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models
{
    /// Implementeer soft-delete 
    public abstract class BasisEntiteit
    {

        /// Primary key entiteit
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Aangemaakt op")]
        public DateTime AangemaaktOp { get; set; } = DateTime.Now;


        /// Datum en tijdstip van de laatste wijziging aan de entiteit en Null indien nog nooit gewijzigd

        [Display(Name = "Gewijzigd op")]
        public DateTime? GewijzigdOp { get; set; }


        ///Datum wanneer entiteit werd "verwijderd

        [Required]
        [Display(Name = "Verwijderd")]
        public DateTime Verwijderd { get; set; } = DateTime.MaxValue;
    }
}
