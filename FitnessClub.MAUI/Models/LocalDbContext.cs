using Microsoft.EntityFrameworkCore;

namespace FitnessClub.MAUI.Models
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options)
            : base(options)
        {
        }

        public DbSet<LocalUser> Users { get; set; } = null!;
        public DbSet<LocalLes> Lessen { get; set; } = null!;
        public DbSet<LocalInschrijving> Inschrijvingen { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite("Filename=local.db");
            }
        }
    }
}