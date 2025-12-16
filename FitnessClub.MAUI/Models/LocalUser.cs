using System.ComponentModel.DataAnnotations;

namespace FitnessClub.MAUI.Models
{
    public class LocalUser
    {
        [Key]
        public string Id { get; set; } = string.Empty; // 👈 string ipv int

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

        // 👇 DIT IS WAT JE MIST - AbonnementType ipv Abonnement
        [MaxLength(50)]
        public string AbonnementType { get; set; } = "Standaard";

        public DateTime LidSinds { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string Rol { get; set; } = "Gebruiker";
    }
}