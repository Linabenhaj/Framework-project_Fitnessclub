using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Models.Data
{
    public class FitnessClubDbContext : IdentityDbContext<Gebruiker>
    {
        public FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options)
            : base(options)
        {
        }

        // Identity gebruikers
        public DbSet<Gebruiker> Gebruikers { get; set; }

        // Abonnementen
        public DbSet<Abonnement> Abonnementen { get; set; }

        // Lessen en inschrijvingen
        public DbSet<Les> Lessen { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision
            builder.Entity<Abonnement>()
                .Property(a => a.Prijs)
                .HasPrecision(18, 2);

            // BELANGRIJK: Configureer ALLEEN DEZE enige relatie
            builder.Entity<Gebruiker>()
                .HasOne(g => g.Abonnement)
                .WithMany()  // CRUCIAAL: Geen navigatie terug!
                .HasForeignKey(g => g.AbonnementId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // BLOKKEER dat Abonnement een Gebruikers collectie heeft
            builder.Entity<Abonnement>()
                .Ignore(a => a.Gebruikers);  // Dit voorkomt de tweede FK!

            // Inschrijving relaties
            builder.Entity<Inschrijving>()
                .HasOne(i => i.Les)
                .WithMany(l => l.Inschrijvingen)
                .HasForeignKey(i => i.LesId);

            builder.Entity<Inschrijving>()
                .HasOne(i => i.Gebruiker)
                .WithMany(g => g.Inschrijvingen)
                .HasForeignKey(i => i.GebruikerId);
        }
    }
}
