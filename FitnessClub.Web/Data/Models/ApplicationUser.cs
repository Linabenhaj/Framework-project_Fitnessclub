using Microsoft.AspNetCore.Identity;

namespace FitnessClub.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }
        public string? Rol { get; set; } // "Admin", "Trainer", "User"
    }
}