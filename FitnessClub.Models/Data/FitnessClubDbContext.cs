using FitnessClub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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

        // LOGIN METHODE VOOR DEMO TESTEN
        public Gebruiker SimpleLogin(string email, string password)
        {
            try
            {
                var user = Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    
                    if (email == "admin@fitness.com" && password == "Admin123!") return user;
                    if (email == "demo@fitness.com" && password == "demo123") return user;
                    if (email == "lid@fitness.com" && password == "Lid123!") return user;

                    
                    if (password == "wachtwoord") return user;
                }

                return null;
            }
            catch (Exception ex)

            {
                // error logging
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }


        public bool EmailExists(string email)
        {
            try
            {
                return Users.Any(u => u.Email == email);
            }


            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EmailExists error: {ex.Message}");
                return false;
            }
        }
    }
}