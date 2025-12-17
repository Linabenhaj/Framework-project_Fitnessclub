using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.Models.Data
{
    public class FitnessClubDbContext : IdentityDbContext<Gebruiker>, IFitnessClubDbContext
    {
        public FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Abonnement> Abonnementen { get; set; }
        public DbSet<Les> Lessen { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Abonnement>()
                .Property(a => a.Prijs)
                .HasPrecision(18, 2);

            builder.Entity<Gebruiker>()
                .HasOne(g => g.Abonnement)
                .WithMany()
                .HasForeignKey(g => g.AbonnementId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.Entity<Abonnement>()
                .Ignore(a => a.Gebruikers);

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