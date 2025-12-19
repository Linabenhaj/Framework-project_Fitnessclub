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
            await context.Database.EnsureCreatedAsync();

            string[] roles = { "Admin", "Trainer", "Lid", "PremiumLid" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            if (!context.Abonnementen.Any())
            {
                var abonnementen = new[]
                {
                    new Abonnement { Naam="Basis", Type="Basis", Prijs=29.99m, DuurInMaanden=1, Beschrijving="Basis toegang", IsActief=true },
                    new Abonnement { Naam="Premium", Type="Premium", Prijs=49.99m, DuurInMaanden=1, Beschrijving="Volledige toegang", IsActief=true },
                    new Abonnement { Naam="Student", Type="Student", Prijs=19.99m, DuurInMaanden=1, Beschrijving="Studentenkorting", IsActief=true }
                };

                await context.Abonnementen.AddRangeAsync(abonnementen);
                await context.SaveChangesAsync();
            }

            var adminEmail = "admin@fitnessclub.be";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new Gebruiker
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Voornaam = "Admin",
                    Achternaam = "Fitness",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Trainer" });
            }

            // Voeg andere demo accounts toe:
            var trainerEmail = "trainer@fitnessclub.be";
            if (await userManager.FindByEmailAsync(trainerEmail) == null)
            {
                var trainer = new Gebruiker
                {
                    UserName = trainerEmail,
                    Email = trainerEmail,
                    Voornaam = "Trainer",
                    Achternaam = "Coach",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(trainer, "Trainer123!");
                await userManager.AddToRoleAsync(trainer, "Trainer");
            }

            var lidEmail = "lid@fitnessclub.be";
            if (await userManager.FindByEmailAsync(lidEmail) == null)
            {
                var lid = new Gebruiker
                {
                    UserName = lidEmail,
                    Email = lidEmail,
                    Voornaam = "Lid",
                    Achternaam = "Gebruiker",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(lid, "Lid123!");
                await userManager.AddToRoleAsync(lid, "Lid");
            }
        }
    }
}