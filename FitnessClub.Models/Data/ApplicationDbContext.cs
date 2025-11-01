using FitnessClub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Models.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;MultipleActiveResultSets=true");
            }
        }

        // DbSets
        public DbSet<Lid> Leden { get; set; }
        public DbSet<Abonnement> Abonnementen { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }
        public DbSet<Betaling> Betalingen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft-delete filters
            modelBuilder.Entity<Lid>().HasQueryFilter(l => l.Verwijderd == DateTime.MaxValue);
            modelBuilder.Entity<Abonnement>().HasQueryFilter(a => a.Verwijderd == DateTime.MaxValue);
            modelBuilder.Entity<Inschrijving>().HasQueryFilter(i => i.Verwijderd == DateTime.MaxValue);
            modelBuilder.Entity<Betaling>().HasQueryFilter(b => b.Verwijderd == DateTime.MaxValue);


            // Relaties
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
    }
}