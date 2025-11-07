using FitnessClub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessClub.Models.Data
{
    public class FitnessClubDbContext : IdentityDbContext<Gebruiker>
    {
        public FitnessClubDbContext() { }

        public FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options) : base(options) { }

        public DbSet<Abonnement> Abonnementen { get; set; }
        public DbSet<Les> Lessen { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SOFT-DELETE QUERY FILTERS
            modelBuilder.Entity<Abonnement>().HasQueryFilter(a => !a.IsVerwijderd);
            modelBuilder.Entity<Les>().HasQueryFilter(l => !l.IsVerwijderd);
            modelBuilder.Entity<Inschrijving>().HasQueryFilter(i => !i.IsVerwijderd);
            modelBuilder.Entity<Gebruiker>().HasQueryFilter(g => !g.IsVerwijderd);

            // RELATIONSHIPS
            modelBuilder.Entity<Inschrijving>()
                .HasOne(i => i.Gebruiker)
                .WithMany(g => g.Inschrijvingen)
                .HasForeignKey(i => i.GebruikerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inschrijving>()
                .HasOne(i => i.Les)
                .WithMany(l => l.Inschrijvingen)
                .HasForeignKey(i => i.LesId)
                .OnDelete(DeleteBehavior.Cascade);

            // DECIMAL PRECISION 
            modelBuilder.Entity<Abonnement>()
                .Property(a => a.Prijs)
                .HasPrecision(10, 2);
        }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BasisEntiteit entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entity.IsVerwijderd = true;
                            entity.VerwijderdOp = DateTime.UtcNow;
                            break;
                        case EntityState.Added:
                            entity.AangemaaktOp = DateTime.UtcNow;
                            entity.IsVerwijderd = false;
                            break;
                        case EntityState.Modified:
                            entity.GewijzigdOp = DateTime.UtcNow;
                            break;
                    }
                }
            }
        }
    }
}