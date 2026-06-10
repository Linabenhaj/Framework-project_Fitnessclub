using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "E-mail is verplicht")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Wachtwoord { get; set; } = "";

        [Display(Name = "Onthou mij")]
        public bool Onthouden { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Voornaam is verplicht")]
        [Display(Name = "Voornaam")]
        public string Voornaam { get; set; } = "";

        [Required(ErrorMessage = "Achternaam is verplicht")]
        [Display(Name = "Achternaam")]
        public string Achternaam { get; set; } = "";

        [Required(ErrorMessage = "E-mail is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        [Display(Name = "Telefoonnummer")]
        [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
        public string? Telefoonnummer { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [StringLength(100, ErrorMessage = "Het {0} moet minstens {2} karakters hebben", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Wachtwoord { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Bevestig wachtwoord")]
        [Compare("Wachtwoord", ErrorMessage = "De wachtwoorden komen niet overeen")]
        public string BevestigWachtwoord { get; set; } = "";

        [Display(Name = "Abonnement")]
        public int? AbonnementId { get; set; }

        // Validatie gebeurt in de Controller (Range op bool is buggy in MVC)
        [Display(Name = "Ik ga akkoord met de voorwaarden")]
        public bool AccepteerVoorwaarden { get; set; }
    }
}