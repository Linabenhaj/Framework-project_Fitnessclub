using FitnessClub.Models.Models;
using System.ComponentModel.DataAnnotations;

public class Lid
{
    public int Id { get; set; }

    [Required]
    public string Voornaam { get; set; }

    [Required]
    public string Achternaam { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public DateTime Geboortedatum { get; set; }

    public string Telefoon { get; set; }

   
    public DateTime LidSinds { get; set; } = DateTime.Now;

    // Foreign key
    public int? AbonnementId { get; set; }

    // SOFT-DELETE 
    public bool IsVerwijderd { get; set; } = false;
    public DateTime? VerwijderdOp { get; set; }

    // Navigatie
    public virtual Abonnement Abonnement { get; set; }
    public virtual ICollection<Inschrijving> Inschrijvingen { get; set; } = new List<Inschrijving>();
    public virtual ICollection<Betaling> Betalingen { get; set; } = new List<Betaling>();
}