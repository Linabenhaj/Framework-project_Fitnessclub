using System;
using System.ComponentModel.DataAnnotations;

namespace FitnesclubLedenbeheer.Models
{
    public class Lid : BasisEntiteit
    {
        public int LidId { get; set; }

        [Required]
        public string Naam { get; set; }

        [Required]
        public string Voornaam { get; set; }

        [Required]
        public DateTime Geboortedatum { get; set; }

        public string Contactgegevens { get; set; }

        public DateTime Inschrijfdatum { get; set; } = DateTime.Now;
    }
}