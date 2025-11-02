using System;

namespace FitnessClub.Models.Models
{
    public abstract class BasisEntiteit
    {
        public bool IsVerwijderd { get; set; } = false;
        public DateTime AangemaaktOp { get; set; } = DateTime.Now;
        public DateTime? GewijzigdOp { get; set; }
    }
}