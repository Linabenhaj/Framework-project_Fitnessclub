// FitnessClub.Models/Data/FitnessClubDbContext.cs
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessClub.Models.Data
{
    public class FitnessClubDbContext : IdentityDbContext<Gebruiker>
    {
        public FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Abonnement> Abonnementen { get; set; }
        public DbSet<Les> Lessen { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Gebruiker
            modelBuilder.Entity<Gebruiker>(entity =>
            {
                entity.Property(g => g.Voornaam).IsRequired().HasMaxLength(100);
                entity.Property(g => g.Achternaam).IsRequired().HasMaxLength(100);
                entity.Property(g => g.Rol).HasMaxLength(50);
                entity.Property(g => g.Geboortedatum).IsRequired();

                // Ignore calculated properties
                entity.Ignore(g => g.DisplayNaam);
                entity.Ignore(g => g.KorteNaam);
                entity.Ignore(g => g.Leeftijd);
                entity.Ignore(g => g.IsVolwassen);
                entity.Ignore(g => g.IsEmailBevestigd);
            });

            // Configure Abonnement
            modelBuilder.Entity<Abonnement>(entity =>
            {
                entity.Property(a => a.Naam).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Omschrijving).HasMaxLength(500);
                entity.Property(a => a.Prijs).HasPrecision(10, 2);
            });

            // Configure Les
            modelBuilder.Entity<Les>(entity =>
            {
                entity.Property(l => l.Naam).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Beschrijving).HasMaxLength(1000);
                entity.Property(l => l.Locatie).HasMaxLength(200);
                entity.Property(l => l.Trainer).HasMaxLength(100);

                // Ignore calculated properties
                entity.Ignore(l => l.DisplayInfo);
                entity.Ignore(l => l.KorteInfo);
                entity.Ignore(l => l.Duur);
                entity.Ignore(l => l.IsToekomstig);
                entity.Ignore(l => l.IsBezig);
                entity.Ignore(l => l.IsVerleden);
                entity.Ignore(l => l.BeschikbarePlaatsen);
                entity.Ignore(l => l.IsVol);
                entity.Ignore(l => l.DagVanWeek);
                entity.Ignore(l => l.TijdRange);
            });

            // Configure Inschrijving
            modelBuilder.Entity<Inschrijving>(entity =>
            {
                entity.Property(i => i.Status).HasMaxLength(50);

                entity.HasOne(i => i.Gebruiker)
                    .WithMany(g => g.Inschrijvingen)
                    .HasForeignKey(i => i.GebruikerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Les)
                    .WithMany(l => l.Inschrijvingen)
                    .HasForeignKey(i => i.LesId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Relationships
            modelBuilder.Entity<Gebruiker>()
                .HasOne(g => g.Abonnement)
                .WithMany()
                .HasForeignKey(g => g.AbonnementId)
                .OnDelete(DeleteBehavior.SetNull);

            // Soft delete query filters
            modelBuilder.Entity<Abonnement>().HasQueryFilter(a => !a.IsVerwijderd);
            modelBuilder.Entity<Les>().HasQueryFilter(l => !l.IsVerwijderd);
            modelBuilder.Entity<Inschrijving>().HasQueryFilter(i => !i.IsVerwijderd);
            modelBuilder.Entity<Gebruiker>().HasQueryFilter(g => !g.IsVerwijderd);
        }

        // Override SaveChanges for automatic timestamps
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BasisEntiteit &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BasisEntiteit)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.AangemaaktOp = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.GewijzigdOp = DateTime.UtcNow;
                }
            }
        }
    }
}
