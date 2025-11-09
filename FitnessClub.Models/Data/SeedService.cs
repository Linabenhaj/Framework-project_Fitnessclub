using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessClub.Models.Data
{
    public class SeedService
    {
        private readonly FitnessClubDbContext _context;
        private readonly UserManager<Gebruiker> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(FitnessClubDbContext context, UserManager<Gebruiker> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public async Task SeedAsync()
        {
            await SeedRolesAndAdmin();
            await SeedAbonnementen();
            await SeedLessen();
            await SeedDemoGebruikers();
            await UpdateExistingUsersWithRoles();
        }

        private async Task SeedRolesAndAdmin()
        {
            // rollen 
            string[] roles = { "Admin", "Lid" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

          
            var adminEmail = "admin@fitness.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new Gebruiker
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Voornaam = "Admin",
                    Achternaam = "FitnessClub",
                    Geboortedatum = new DateTime(1980, 1, 1),
                    Telefoon = "0123456789",
                    Rol = "Admin"
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                adminUser.Rol = "Admin";
                await _userManager.UpdateAsync(adminUser);
            }

          
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
            if (!await _context.Lessen.AnyAsync())
            {
                var lessen = new[]
                {
                    new Les
                    {
                        Naam = "Yoga Basics",
                        Beschrijving = "Ontspannende yoga voor beginners",
                        StartTijd = DateTime.Today.AddDays(1).AddHours(18),
                        EindTijd = DateTime.Today.AddDays(1).AddHours(19),
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

                await _context.Lessen.AddRangeAsync(lessen);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedDemoGebruikers()
        {
            var basicAbonnement = await _context.Abonnementen.FirstAsync(a => a.Naam == "Basic");

            var lidEmail = "lid@fitness.com";
            var lidUser = await _userManager.FindByEmailAsync(lidEmail);

            if (lidUser == null)
            {
                lidUser = new Gebruiker
                {
                    Voornaam = "Demo",
                    Achternaam = "Lid",
                    UserName = lidEmail,
                    Email = lidEmail,
                    Telefoon = "0123456789",
                    Geboortedatum = new DateTime(1990, 1, 1),
                    AbonnementId = basicAbonnement.Id,
                    Rol = "Lid"
                };

                var result = await _userManager.CreateAsync(lidUser, "Lid123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(lidUser, "Lid");
                }
            }
            else
            {
                lidUser.Rol = "Lid";
                await _userManager.UpdateAsync(lidUser);
            }
        }

        private async Task UpdateExistingUsersWithRoles()
        {
            try
            {
                var usersWithoutRole = _context.Users.Where(u => string.IsNullOrEmpty(u.Rol)).ToList();

                foreach (var user in usersWithoutRole)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any())
                    {
                        user.Rol = roles.First();
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij updaten roles: {ex.Message}");
            }
        }

        public List<Abonnement> GetGoedkopeAbonnementen()
        {
            var goedkopeAbonnementen = from abonnement in _context.Abonnementen
                                       where abonnement.Prijs < 50m
                                       orderby abonnement.Prijs
                                       select abonnement;

            return goedkopeAbonnementen.ToList();
        }
    }
}