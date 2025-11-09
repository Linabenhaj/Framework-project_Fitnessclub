using Microsoft.EntityFrameworkCore;
using System.Windows;
using FitnessClub.Models.Data;
using System.Linq;

namespace FitnessClub.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    context.Database.EnsureCreated();
                    SeedInitialData(context);
                }

                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij opstarten applicatie: {ex.Message}", "Startup Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                // Toch doorgaan met applicatie
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        void SeedInitialData(FitnessClubDbContext context)
        {
            try
            {
                // Seed abonnementen 
                if (!context.Abonnementen.Any())
                {
                    var abonnementen = new[]
                    {
                        new Models.Abonnement { Naam = "Starter", Prijs = 19.99m, Omschrijving = "Perfect voor beginners - basis toegang", LooptijdMaanden = 1 },
                        new Models.Abonnement { Naam = "Premium", Prijs = 39.99m, Omschrijving = "All-inclusive - toegang tot alle lessen", LooptijdMaanden = 1 },
                        new Models.Abonnement { Naam = "VIP", Prijs = 59.99m, Omschrijving = "VIP behandeling + persoonlijke trainer", LooptijdMaanden = 1 }
                    };
                    context.Abonnementen.AddRange(abonnementen);
                    context.SaveChanges();
                }

                // Seed admin gebruiker
                if (!context.Users.Any(u => u.Email == "admin@fitness.com"))
                {
                    var adminUser = new Models.Gebruiker
                    {
                        UserName = "admin@fitness.com",
                        Email = "admin@fitness.com",
                        Voornaam = "Admin",
                        Achternaam = "User",
                        Telefoon = "0123456789",
                        Geboortedatum = new System.DateTime(1980, 1, 1)
                    };
                    context.Users.Add(adminUser);
                }

                // Seed demo lid
                if (!context.Users.Any(u => u.Email == "lid@fitness.com"))
                {
                    var lidUser = new Models.Gebruiker
                    {
                        UserName = "lid@fitness.com",
                        Email = "lid@fitness.com",
                        Voornaam = "Demo",
                        Achternaam = "Lid",
                        Telefoon = "0987654321",
                        Geboortedatum = new System.DateTime(1990, 1, 1)
                    };
                    context.Users.Add(lidUser);
                }

                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                // Toon fout maar ga door
                System.Diagnostics.Debug.WriteLine($"Seed data fout: {ex.Message}");
            }
        }
    }
}