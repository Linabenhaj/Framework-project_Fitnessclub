using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public class Gebruiker : IdentityUser
    {
        [Required]
        public string Voornaam { get; set; }

        [Required]
        public string Achternaam { get; set; }

        [Required]
        public DateTime Geboortedatum { get; set; }

        // Soft-delete
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }
    }
}