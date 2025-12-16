using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub.Models.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            FitnessClubDbContext context,
            UserManager<Gebruiker> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Zorg dat database bestaat
            await context.Database.EnsureCreatedAsync();

            // Rollen
            string[] roles = { "Admin", "Trainer", "Lid", "PremiumLid" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Abonnementen
            if (!context.Abonnementen.Any())
            {
                var abonnementen = new[]
                {
                    new Abonnement { Naam="Basis", Type="Basis", Prijs=29.99m, DuurInMaanden=1, Beschrijving="Basis toegang tot fitness apparaten", IsActief=true },
                    new Abonnement { Naam="Premium", Type="Premium", Prijs=49.99m, DuurInMaanden=1, Beschrijving="Volledige toegang tot alle faciliteiten", IsActief=true },
                    new Abonnement { Naam="Student", Type="Student", Prijs=19.99m, DuurInMaanden=1, Beschrijving="Korting voor studenten", IsActief=true }
                };

                await context.Abonnementen.AddRangeAsync(abonnementen);
                await context.SaveChangesAsync();
            }

            // Admin gebruiker
            var adminEmail = "admin@fitnessclub.be";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new Gebruiker
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Voornaam = "Admin",
                    Achternaam = "Fitness",
                    Geboortedatum = new DateTime(1980, 1, 1),
                    EmailConfirmed = true,
                    AangemaaktOp = DateTime.UtcNow
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Trainer" });

                var basis = await context.Abonnementen.FirstOrDefaultAsync(a => a.Type == "Basis");
                if (basis != null)
                {
                    adminUser.AbonnementId = basis.Id;
                    await userManager.UpdateAsync(adminUser);
                }
            }
        }
    }
}
