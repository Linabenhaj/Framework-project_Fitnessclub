using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessClub.Models;

namespace FitnessClub.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Database tabellen
        public DbSet<Lid> Leden { get; set; }
        public DbSet<Abonnement> Abonnementen { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }
        public DbSet<Betaling> Betalingen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Eenvoudige connection string zonder User Secrets
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // relaties 
            modelBuilder.Entity<Inschrijving>()
                .HasOne(i => i.Lid)
                .WithMany(l => l.Inschrijvingen)
                .HasForeignKey(i => i.LidId);

            modelBuilder.Entity<Inschrijving>()
                .HasOne(i => i.Abonnement)
                .WithMany(a => a.Inschrijvingen)
                .HasForeignKey(i => i.AbonnementId);

            modelBuilder.Entity<Betaling>()
                .HasOne(b => b.Inschrijving)
                .WithMany(i => i.Betalingen)
                .HasForeignKey(b => b.InschrijvingId);
        }

        //  seeder zonder Dummy 
        public static void Seeder(ApplicationDbContext context)
        {
            //  test data
            if (!context.Leden.Any())
            {
                context.Leden.AddRange(Lid.SeedingData());
                context.SaveChanges();
            }

            if (!context.Abonnementen.Any())
            {
                context.Abonnementen.AddRange(Abonnement.SeedingData());
                context.SaveChanges();
            }

            if (!context.Inschrijvingen.Any())
            {
                context.Inschrijvingen.AddRange(Inschrijving.SeedingData());
                context.SaveChanges();
            }

            if (!context.Betalingen.Any())
            {
                context.Betalingen.AddRange(Betaling.SeedingData());
                context.SaveChanges();
            }
        }
    }
}