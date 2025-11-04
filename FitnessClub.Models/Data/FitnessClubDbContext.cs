using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Models.Data
{
    public class FitnessClubDbContext : IdentityDbContext<Gebruiker>
    {
        public FitnessClubDbContext() { }

        public FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;MultipleActiveResultSets=true");
            }
        }

        public DbSet<Lid> Leden { get; set; }
        public DbSet<Abonnement> Abonnementen { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }
        public DbSet<Betaling> Betalingen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DECIMAL PRECISION FIXES
            modelBuilder.Entity<Abonnement>()
                .Property(a => a.Prijs)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Betaling>()
                .Property(b => b.Bedrag)
                .HasPrecision(10, 2);

            // SOFT-DELETE 
            modelBuilder.Entity<Lid>().HasQueryFilter(l => !l.IsVerwijderd);
            modelBuilder.Entity<Abonnement>().HasQueryFilter(a => !a.IsVerwijderd);
            modelBuilder.Entity<Inschrijving>().HasQueryFilter(i => !i.IsVerwijderd);
            modelBuilder.Entity<Betaling>().HasQueryFilter(b => !b.IsVerwijderd);
            modelBuilder.Entity<Gebruiker>().HasQueryFilter(g => !g.IsVerwijderd);
        }
    }
}