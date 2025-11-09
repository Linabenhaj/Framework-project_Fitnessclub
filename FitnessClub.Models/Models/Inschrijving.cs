using System;

namespace FitnessClub.Models
{
    public class Inschrijving : BasisEntiteit
    {
        public new int Id { get; set; }  
        public DateTime InschrijfDatum { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public string GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }

        public int LesId { get; set; }
        public Les Les { get; set; }
    }
}