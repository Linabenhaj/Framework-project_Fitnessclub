using System;
using System.ComponentModel.DataAnnotations;

namespace FitnesclubLedenbeheer.Models
{
    public abstract class BasisEntiteit
    {
        public int Id { get; set; }
        public DateTime AangemaaktOp { get; set; } = DateTime.Now;
        public bool IsVerwijderd { get; set; } = false;
    }
}