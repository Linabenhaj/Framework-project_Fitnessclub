using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models.Models
{
    // Model voor lessen in de MAUI app
    public class LocalLes
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Naam { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Beschrijving { get; set; } = string.Empty;

        [Required]
        public DateTime StartTijd { get; set; }

        [Required]
        public DateTime EindTijd { get; set; }

        [Range(1, 100)]
        public int MaxDeelnemers { get; set; } = 20;

        [StringLength(200)]
        public string Locatie { get; set; } = string.Empty;

        [StringLength(100)]
        public string Trainer { get; set; } = string.Empty;

        public bool IsActief { get; set; } = true;

        public DateTime? LastSynced { get; set; }

        // Navigation property voor inschrijvingen
        public List<LocalInschrijving> Inschrijvingen { get; set; } = new();

        public string DisplayNaam => $"{Naam} ({StartTijd:HH:mm})";
        public bool IsToekomstig => StartTijd > DateTime.Now;
        public bool IsBezig => DateTime.Now >= StartTijd && DateTime.Now <= EindTijd;
        public bool IsVerleden => EindTijd < DateTime.Now;
        public int AantalIngeschreven => Inschrijvingen?.Count ?? 0;
        public int BeschikbarePlaatsen => MaxDeelnemers - AantalIngeschreven;
        public bool IsVol => BeschikbarePlaatsen <= 0;

        [NotMapped]
        public bool IsIngeschrevenDoorMij { get; set; }

        [NotMapped]
        public bool IsNietIngeschrevenDoorMij => !IsIngeschrevenDoorMij;
    }
}
