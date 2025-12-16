using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessClub.Models.Models
{
    public abstract class BasisEntiteit
    {
        [Key]
        public int Id { get; set; }

        // Soft delete
        public bool IsVerwijderd { get; set; } = false;
        public DateTime? VerwijderdOp { get; set; }

        // Audit
        public DateTime AangemaaktOp { get; set; } = DateTime.UtcNow;
        public DateTime? GewijzigdOp { get; set; }

        // Specifiek voor Abonnement of andere entiteiten
        public int LooptijdMaanden { get; set; } = 12;

       
    }
}