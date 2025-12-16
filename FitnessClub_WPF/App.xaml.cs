using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            // Configureer Dependency Injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext
            services.AddDbContext<FitnessClubDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<FitnessClubDbContext>();

                    // Create database if not exists
                    context.Database.EnsureCreated();

                    // Seed initial data
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
                        new Abonnement {
                            Naam = "Starter",
                            Prijs = 19.99m,
                            Omschrijving = "Perfect voor beginners - basis toegang",
                            LooptijdMaanden = 1,
                            IsActief = true
                        },
                        new Abonnement {
                            Naam = "Premium",
                            Prijs = 39.99m,
                            Omschrijving = "All-inclusive - toegang tot alle lessen",
                            LooptijdMaanden = 1,
                            IsActief = true
                        },
                        new Abonnement {
                            Naam = "VIP",
                            Prijs = 59.99m,
                            Omschrijving = "VIP behandeling + persoonlijke trainer",
                            LooptijdMaanden = 1,
                            IsActief = true
                        }
                    };
                    context.Abonnementen.AddRange(abonnementen);
                    context.SaveChanges();
                }

                // Seed admin gebruiker (zonder Identity voor nu)
                if (!context.Users.Any(u => u.Email == "admin@fitness.com"))
                {
                    var adminUser = new Gebruiker
                    {
                        UserName = "admin@fitness.com",
                        Email = "admin@fitness.com",
                        Voornaam = "Admin",
                        Achternaam = "User",
                        PhoneNumber = "0123456789",
                        Geboortedatum = new System.DateTime(1980, 1, 1),
                        Rol = "Admin"
                    };
                    context.Users.Add(adminUser);
                }

                // Seed demo lid
                if (!context.Users.Any(u => u.Email == "lid@fitness.com"))
                {
                    var lidUser = new Gebruiker
                    {
                        UserName = "lid@fitness.com",
                        Email = "lid@fitness.com",
                        Voornaam = "Demo",
                        Achternaam = "Lid",
                        PhoneNumber = "0987654321",
                        Geboortedatum = new System.DateTime(1990, 1, 1),
                        Rol = "Lid"
                    };
                    context.Users.Add(lidUser);
                }

                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Seed data fout: {ex.Message}");
            }
        }
    }
}