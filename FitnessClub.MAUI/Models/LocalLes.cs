using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.MAUI.Models
{
    public class LocalLes
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, MaxLength(200)]
        public string Titel { get; set; } = string.Empty;

 
        public string Naam { get; set; } = string.Empty;
        public string Trainer { get; set; } = string.Empty;
        public string Beschrijving { get; set; } = string.Empty;
        public string Locatie { get; set; } = string.Empty;
        public bool IsActief { get; set; } = true;
        public int MaxDeelnemers { get; set; } = 20;
        public int MaxAantalDeelnemers { get; set; } = 20; 

        public DateTime StartTijd { get; set; } = DateTime.Now;
        public DateTime EindTijd { get; set; } = DateTime.Now.AddHours(1);

        public virtual ICollection<LocalInschrijving> Inschrijvingen { get; set; } = new List<LocalInschrijving>();
    }
}