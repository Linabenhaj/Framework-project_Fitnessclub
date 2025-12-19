using System;

namespace FitnessClub.MAUI.Models
{
    public class LocalInschrijving
    {
        public int Id { get; set; }

       
        public string GebruikerId { get; set; } = string.Empty;

        public int LesId { get; set; }
        public DateTime InschrijfDatum { get; set; }
        public string Status { get; set; } = "Actief"; // Actief, Geannuleerd, Voltooid
        public DateTime? LastSynced { get; set; }

        // Navigation properties
        public LocalLes? Les { get; set; }

        //gebruiker info nodig hebt
        public string? GebruikerNaam { get; set; }
        public string? GebruikerEmail { get; set; }
    }
}