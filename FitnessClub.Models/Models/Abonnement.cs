// FitnessClub.Models/Models/Abonnement.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Abonnement : BasisEntiteit
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        public string Naam { get; set; } = string.Empty;

        [Display(Name = "Prijs")]
        [DataType(DataType.Currency)]
        public decimal Prijs { get; set; }

        [Display(Name = "Omschrijving")]
        public string Omschrijving { get; set; } = string.Empty;

        [Display(Name = "Looptijd (maanden)")]
        public int LooptijdMaanden { get; set; } = 1;

        [Display(Name = "Actief")]
        public bool IsActief { get; set; } = true;

        
    }
}