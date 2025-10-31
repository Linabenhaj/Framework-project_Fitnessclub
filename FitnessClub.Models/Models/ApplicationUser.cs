using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models
{
   
    /// Gebruikt voor authenticatie en autorisatie in de fitness club applicatie
    public class ApplicationUser : IdentityUser
    {

        /// Voornaam van de gebruiker
     
        [Required]
        [Display(Name = "Voornaam")]
        [StringLength(50)]
        public string Voornaam { get; set; } = string.Empty;

     
        /// Achternaam van de gebruiker
        [Required]
        [Display(Name = "Achternaam")]
        [StringLength(50)]
        public string Achternaam { get; set; } = string.Empty;

   
        /// Datum wanneer de gebruiker lid werd van de fitness club
        
        [Required]
        [Display(Name = "Lid sinds")]
        public DateTime LidSinds { get; set; } = DateTime.Now;

        
        /// Dummy gebruiker object gebruikt als placeholder voor niet-bestaande gebruikers
   
       
        public static ApplicationUser Dummy = new ApplicationUser
        {
            Id = "-",
            Voornaam = "-",
            Achternaam = "-",
            UserName = "Dummy",
            NormalizedUserName = "DUMMY",
            Email = "dummy@fitnessclub.be",
            LockoutEnabled = true,
            LockoutEnd = DateTimeOffset.MaxValue
        };

 
        /// Geeft de volledige naam van de gebruiker terug
        public override string ToString()
        {
            return $"{Voornaam} {Achternaam}";
        }

        
        /// Initialiseert de database met standaard gebruikers en rollen
 
        public static async Task Seeder()
        {
            // Implementatie volgt later
        }
    }
}