using Microsoft.EntityFrameworkCore;
using FitnessClub.MAUI.Models;

namespace FitnessClub.MAUI.Models 
{
    public class LocalDbContext : DbContext
    {
        // onstructor met DbContextOptions
        public LocalDbContext(DbContextOptions<LocalDbContext> options)
            : base(options)
        {
        }

      

        public DbSet<LocalUser> Users { get; set; }
        public DbSet<LocalLes> Lessen { get; set; }
        public DbSet<LocalInschrijving> Inschrijvingen { get; set; }

       
    }
}