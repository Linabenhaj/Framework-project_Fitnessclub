using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Les : BasisEntiteit
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        [StringLength(100)]
        public string Naam { get; set; } = string.Empty;

        [Display(Name = "Beschrijving")]
        [StringLength(1000)]
        public string Beschrijving { get; set; } = string.Empty;

        [Required(ErrorMessage = "Starttijd is verplicht")]
        [Display(Name = "Starttijd")]
        [DataType(DataType.DateTime)]
        public DateTime StartTijd { get; set; }

        [Required(ErrorMessage = "Eindtijd is verplicht")]
        [Display(Name = "Eindtijd")]
        [DataType(DataType.DateTime)]
        public DateTime EindTijd { get; set; }

        [Display(Name = "Maximaal deelnemers")]
        [Range(1, 100, ErrorMessage = "Maximaal aantal deelnemers moet tussen 1 en 100 zijn")]
        public int MaxDeelnemers { get; set; } = 20;

        [Display(Name = "Locatie")]
        [StringLength(200)]
        public string Locatie { get; set; } = string.Empty;

        [Display(Name = "Trainer")]
        [StringLength(100)]
        public string Trainer { get; set; } = string.Empty;

        [Display(Name = "Actief")]
        public bool IsActief { get; set; } = true;



        public string KorteInfo => $"{Naam} ({StartTijd:HH:mm})";
        public string DagVanWeek => StartTijd.ToString("dddd");
        public string TijdRange => $"{StartTijd:HH:mm} - {EindTijd:HH:mm}";

        // Navigation property
        public ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();

        //  properties
        public string DisplayInfo => $"{Naam} - {StartTijd:dd/MM/yyyy HH:mm} ({Locatie})";
        public int Duur => (int)(EindTijd - StartTijd).TotalMinutes;
        public bool IsToekomstig => StartTijd > DateTime.Now;
        public bool IsBezig => DateTime.Now >= StartTijd && DateTime.Now <= EindTijd;
        public bool IsVerleden => EindTijd < DateTime.Now;
        public int BeschikbarePlaatsen => MaxDeelnemers - (Inschrijvingen?.Count ?? 0);
        public bool IsVol => BeschikbarePlaatsen <= 0;
    }
}