using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models.Models
{
    public class Abonnement  // VERWIJDER: : BasisEntiteit
    {
        public int Id { get; set; }

        [Required]
        public string Naam { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        public decimal Prijs { get; set; }

        public int DuurInMaanden { get; set; } = 12;
        public string Beschrijving { get; set; } = string.Empty;
        public bool IsActief { get; set; } = true;

        // Voeg deze properties toe 
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }
        public int LooptijdMaanden => DuurInMaanden;


        [NotMapped]  // Zorg dat dit niet in de database komt
        public virtual ICollection<Gebruiker> Gebruikers { get; set; } = new List<Gebruiker>();
    }
}