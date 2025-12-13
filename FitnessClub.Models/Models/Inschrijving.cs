using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Inschrijving : BasisEntiteit
    {
        [Display(Name = "Inschrijfdatum")]
        [DataType(DataType.DateTime)]
        public DateTime InschrijfDatum { get; set; } = DateTime.UtcNow;

        [Display(Name = "Opmerkingen")]
        public string Opmerkingen { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Actief";

        [Required]
        public string GebruikerId { get; set; } = string.Empty;

        [Required]
        public int LesId { get; set; }

        // Navigation properties
        public Gebruiker Gebruiker { get; set; } = null!;
        public Les Les { get; set; } = null!;
    }
}