
using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public abstract class BasisEntiteit
    {
        [Key]
        public int Id { get; set; }

        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }
    }
}