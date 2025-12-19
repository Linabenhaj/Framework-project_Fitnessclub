using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub.Models.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FitnessClubDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<FitnessClubDbContext>>()))
            {
                // Seed Abonnementen
                if (!context.Abonnementen.Any())
                {
                    context.Abonnementen.AddRange(
                        new Abonnement
                        {
                            Naam = "Basis",
                            Type = "Maandelijks",
                            Beschrijving = "Toegang tot basislessen",
                            Prijs = 29.99m,
                            DuurInMaanden = 1,
                            IsActief = true
                        },
                        new Abonnement
                        {
                            Naam = "Premium",
                            Type = "Maandelijks",
                            Beschrijving = "Alles inclusief",
                            Prijs = 49.99m,
                            DuurInMaanden = 1,
                            IsActief = true
                        },
                        new Abonnement
                        {
                            Naam = "Jaar",
                            Type = "Jaarlijks",
                            Beschrijving = "Jaarabonnement",
                            Prijs = 299.99m,
                            DuurInMaanden = 12,
                            IsActief = true
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}