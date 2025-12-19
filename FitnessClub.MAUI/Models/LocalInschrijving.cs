using System;

namespace FitnessClub.MAUI.Models
{
    public class LocalInschrijving
    {
        public int Id { get; set; }

        // ✅ CORRECTIE: Property heet 'GebruikerId', niet 'UserId'
        public string GebruikerId { get; set; } = string.Empty;

        public int LesId { get; set; }
        public DateTime InschrijfDatum { get; set; }
        public string Status { get; set; } = "Actief"; // Actief, Geannuleerd, Voltooid
        public DateTime? LastSynced { get; set; }

        // Navigation properties
        public LocalLes? Les { get; set; }

        // ✅ OPTIONEEL: Als je gebruiker info nodig hebt
        public string? GebruikerNaam { get; set; }
        public string? GebruikerEmail { get; set; }
    }
}