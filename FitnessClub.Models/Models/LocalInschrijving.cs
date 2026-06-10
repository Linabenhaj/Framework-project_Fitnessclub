using System;

namespace FitnessClub.Models.Models
{
    // Afgeleid model van Inschrijving voor lokale opslag (SQLite) in de MAUI-app.
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

        // Optionele gebruikersinformatie
        public string? GebruikerNaam { get; set; }
        public string? GebruikerEmail { get; set; }
    }
}
