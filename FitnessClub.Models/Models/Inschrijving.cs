using System;

namespace FitnessClub.Models
{
    public class Inschrijving : BasisEntiteit
    {
        public DateTime InschrijfDatum { get; set; } = DateTime.UtcNow;

        // Start en eind datum 
        public DateTime StartDatum { get; set; } = DateTime.UtcNow;
        public DateTime? EindDatum { get; set; }

        // Foreign keys
        public string GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }

        public int LesId { get; set; }
        public Les Les { get; set; }
    }
}