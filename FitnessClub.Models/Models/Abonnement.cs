using System;
using System.ComponentModel.DataAnnotations;

namespace FitnesclubLedenbeheer.Models
{
    public class Abonnement : BasisEntiteit
    {
        public int AbonnementId { get; set; }

        [Required]
        public string Naam { get; set; }

        public string Omschrijving { get; set; }

        [Required]
        public decimal MaandelijksePrijs { get; set; }

        [Required]
        public int LooptijdMaanden { get; set; }

        public string Status { get; set; } = "Actief";
    }
}