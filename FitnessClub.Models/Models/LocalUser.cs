using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    // Afgeleid model van Gebruiker voor lokale opslag (SQLite) in de MAUI-app.
    // Bevat alleen de velden die de mobiele client nodig heeft (geen IdentityUser-overhead).
    public class LocalUser
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Voornaam { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Achternaam { get; set; } = string.Empty;

        public DateTime? Geboortedatum { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        [MaxLength(50)]
        public string AbonnementType { get; set; } = "Standaard";

        public DateTime LidSinds { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string Rol { get; set; } = "Lid";
    }
}
