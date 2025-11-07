using System;

namespace FitnessClub.Models
{
    public abstract class BasisEntiteit
    {
        public int Id { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }
    }
}