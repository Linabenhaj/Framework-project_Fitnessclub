using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace FitnessClub.Models.Data
{
    public class SeedService
    {
        private readonly FitnessClubDbContext _context;

        public SeedService(FitnessClubDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedAbonnementen();
            await SeedLessen();
            await SeedDemoGebruikers();
        }

        private async Task SeedAbonnementen()
        {
            if (!await _context.Abonnementen.AnyAsync())
            {
                var abonnementen = new[]
                {
                    new Abonnement { Naam = "Basic", Prijs = 29.99m, Omschrijving = "Basis abonnement" },
                    new Abonnement { Naam = "Premium", Prijs = 49.99m, Omschrijving = "Premium abonnement" },
                    new Abonnement { Naam = "VIP", Prijs = 79.99m, Omschrijving = "VIP abonnement" }
                };

                await _context.Abonnementen.AddRangeAsync(abonnementen);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedLessen()
        {
            
            if (!context.Lessen.Any())
            {
                var lessen = new[]
                {
        new Les
        {
            Naam = "Yoga Basics",
            Beschrijving = "Ontspannende yoga voor beginners",
            StartTijd = DateTime.Today.AddDays(1).AddHours(18), // Gebruik StartTijd i.p.v. DatumTijd
            EindTijd = DateTime.Today.AddDays(1).AddHours(19),  // Gebruik EindTijd i.p.v. Duur
            MaxDeelnemers = 20
        },
        new Les
        {
            Naam = "High Intensity Training",
            Beschrijving = "Intensieve cardio training",
            StartTijd = DateTime.Today.AddDays(2).AddHours(19),
            EindTijd = DateTime.Today.AddDays(2).AddHours(20),
            MaxDeelnemers = 15
        },
        new Les
        {
            Naam = "Pilates",
            Beschrijving = "Pilates voor core strength",
            StartTijd = DateTime.Today.AddDays(3).AddHours(17),
            EindTijd = DateTime.Today.AddDays(3).AddHours(18),
            MaxDeelnemers = 25
        }
    };
                context.Lessen.AddRange(lessen);
                context.SaveChanges();
            }
        }

        private async Task SeedDemoGebruikers()
        {
            if (!await _context.Users.AnyAsync())
            {
                var basicAbonnement = await _context.Abonnementen.FirstAsync(a => a.Naam == "Basic");

                // demo gebruiker - zonder wachtwoord 
                var demoUser = new Gebruiker
                {
                    Voornaam = "Demo",
                    Achternaam = "Gebruiker",
                    UserName = "demo@fitness.com",
                    Email = "demo@fitness.com",
                    Telefoon = "0123456789",
                    Geboortedatum = new DateTime(1990, 1, 1),
                    AbonnementId = basicAbonnement.Id
                };

                await _context.Users.AddAsync(demoUser);
                await _context.SaveChangesAsync();
            }
        }
    }
}