using System;
namespace FitnessClub.Models.Models
{
    public interface IBasisEntiteit
    {
        int Id { get; set; }
        DateTime AangemaaktOp { get; set; }
        DateTime? GewijzigdOp { get; set; }
        bool IsVerwijderd { get; set; }
        DateTime? VerwijderdOp { get; set; }
    }
}