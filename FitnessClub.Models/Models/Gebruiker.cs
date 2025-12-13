
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Gebruiker : IdentityUser
    {
        [Required(ErrorMessage = "Voornaam is verplicht")]
        [Display(Name = "Voornaam")]
        public string Voornaam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Achternaam is verplicht")]
        [Display(Name = "Achternaam")]
        public string Achternaam { get; set; } = string.Empty;

        [Display(Name = "Geboortedatum")]
        [DataType(DataType.Date)]
        public DateTime Geboortedatum { get; set; } = DateTime.Now.AddYears(-20);

        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        public int? AbonnementId { get; set; }

        // Navigation properties
        public Abonnement? Abonnement { get; set; }
        public ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();

        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }

        // Computed properties
        public string DisplayNaam => $"{Voornaam} {Achternaam}".Trim();
        public string KorteNaam => $"{Voornaam} {Achternaam.Substring(0, 1)}.";

        public int Leeftijd
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - Geboortedatum.Year;
                if (Geboortedatum.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public bool IsVolwassen => Leeftijd >= 18;
        public bool IsEmailBevestigd => EmailConfirmed;

        [Display(Name = "Telefoon")]
        [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
        public string Telefoon
        {
            get => PhoneNumber ?? string.Empty;
            set => PhoneNumber = value;
        }
    }
}