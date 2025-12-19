using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub.Models.Models
{
    public class LogError
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? StackTrace { get; set; }

        [StringLength(500)]
        public string? Source { get; set; }

        [StringLength(100)]
        public string? UserId { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Level { get; set; } = "Error"; 

        [StringLength(500)]
        public string? RequestPath { get; set; }

        [StringLength(50)]
        public string? ClientIp { get; set; }

        [StringLength(500)]
        public string? AdditionalInfo { get; set; }
    }
}