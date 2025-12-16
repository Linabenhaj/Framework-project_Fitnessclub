using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.MAUI.Models
{
    public class LocalInschrijving
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string LesId { get; set; } = string.Empty;

        public DateTime InschrijfDatum { get; set; } = DateTime.Now;

        // navigation properties (optional)
        public virtual LocalUser? User { get; set; }
        public virtual LocalLes? Les { get; set; }
    }
}