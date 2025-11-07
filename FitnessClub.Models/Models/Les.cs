using System;
using System.Collections.Generic;

namespace FitnessClub.Models
{
    public class Les : BasisEntiteit
    {
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime EindTijd { get; set; }
        public int MaxDeelnemers { get; set; }

        // Nieuwe properties 
        public DateTime DatumTijd
        {
            get => StartTijd;
            set => StartTijd = value;
        }

        public int Duur
        {
            get => (int)(EindTijd - StartTijd).TotalMinutes;
            set => EindTijd = StartTijd.AddMinutes(value);
        }

        public ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();
    }
}