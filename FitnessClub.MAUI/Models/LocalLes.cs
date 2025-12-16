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

        public DateTime StartTijd { get; set; } = DateTime.Now;
        public DateTime EindTijd { get; set; } = DateTime.Now.AddHours(1);

        [MaxLength(100)]
        public string Locatie { get; set; } = string.Empty;

        public virtual ICollection<LocalInschrijving> Inschrijvingen { get; set; } = new List<LocalInschrijving>();
    }
}