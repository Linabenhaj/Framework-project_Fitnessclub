using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FitnessClub.MAUI.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.MAUI.Services
{
    public class Synchronizer  // Synchroniseert data tussen API en lokale database
    {
        private readonly LocalDbContext _context;
        private readonly ApiService _apiService;
        private bool _isBusy;

        public bool DatabaseExists { get; private set; }
        public bool IsSynchronizing => _isBusy;

        public Synchronizer(LocalDbContext context, ApiService apiService)
        {
            _context = context;
            _apiService = apiService;
        }

        // Initialiseer database
        public async Task InitializeDatabase()
        {
            try
            {
                await _context.Database.MigrateAsync();  // Voer migraties uit
                await SeedDefaultData();  // Voeg demo data toe
                DatabaseExists = true;
                Debug.WriteLine("Database initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }

        // Voeg demo data toe als database leeg is
        private async Task SeedDefaultData()
        {
            if (!await _context.Lessen.AnyAsync())
            {
                var demoLessen = new[]
                {
                    new LocalLes
                    {
                        Naam = "Yoga Basis",
                        StartTijd = DateTime.Now.AddDays(1).AddHours(10),
                        EindTijd = DateTime.Now.AddDays(1).AddHours(11),
                        Locatie = "Zaal 1",
                        Trainer = "Anna",
                        MaxDeelnemers = 20,
                        Beschrijving = "Beginner yoga sessie",
                        IsActief = true
                    },
                    new LocalLes
                    {
                        Naam = "HIIT Training",
                        StartTijd = DateTime.Now.AddDays(2).AddHours(18),
                        EindTijd = DateTime.Now.AddDays(2).AddHours(19),
                        Locatie = "Zaal 2",
                        Trainer = "Mike",
                        MaxDeelnemers = 15,
                        Beschrijving = "High Intensity Interval Training",
                        IsActief = true
                    }
                };

                await _context.Lessen.AddRangeAsync(demoLessen);
                await _context.SaveChangesAsync();
                Debug.WriteLine("Seeded default data");
            }
        }

        // Login via API en sync data
        public async Task<bool> LoginWithApi(string email, string password)
        {
            try
            {
                var result = await _apiService.LoginAsync(email, password);

                if (result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    General.SaveUserInfo(
                        result.Id ?? Guid.NewGuid().ToString(),
                        result.Email ?? email,
                        result.Voornaam ?? email.Split('@')[0],
                        result.Achternaam ?? "",
                        result.Roles?.FirstOrDefault() ?? "Gebruiker",
                        result.Token
                    );

                    _apiService.SetToken(result.Token);

                    await SynchronizeAll();  // Sync na succesvolle login

                    Debug.WriteLine($"User {email} logged in successfully via API");
                    return true;
                }

                Debug.WriteLine($"API login failed: {result.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"API Login error: {ex.Message}");
                return false;
            }
        }

        // Controleer of gebruiker geauthoriseerd is
        public async Task<bool> IsAuthorized()
        {
            if (!string.IsNullOrEmpty(General.Token))
            {
                _apiService.SetToken(General.Token);
                return await _apiService.ValidateTokenAsync();
            }
            return false;
        }

        // Synchroniseer alle data
        public async Task SynchronizeAll()
        {
            if (_isBusy) return;
            _isBusy = true;

            try
            {
                Debug.WriteLine("🔄 Starting REAL API synchronization...");

                // 1. Synchroniseer lessen
                var lessenResponse = await _apiService.GetAllLessenAsync();

                if (lessenResponse.Success && lessenResponse.Data != null)
                {
                    Debug.WriteLine($"📋 Received {lessenResponse.Data.Count} lessons from API");

                    foreach (var apiLes in lessenResponse.Data)
                    {
                        var existingLes = await _context.Lessen
                            .Include(l => l.Inschrijvingen)
                            .FirstOrDefaultAsync(l => l.Id == apiLes.Id);

                        if (existingLes == null)
                        {
                            _context.Lessen.Add(apiLes);  // Voeg nieuwe les toe
                            Debug.WriteLine($"➕ Added new lesson: {apiLes.Naam}");
                        }
                        else
                        {
                            // Update bestaande les
                            existingLes.Naam = apiLes.Naam;
                            existingLes.Beschrijving = apiLes.Beschrijving;
                            existingLes.StartTijd = apiLes.StartTijd;
                            existingLes.EindTijd = apiLes.EindTijd;
                            existingLes.Locatie = apiLes.Locatie;
                            existingLes.Trainer = apiLes.Trainer;
                            existingLes.MaxDeelnemers = apiLes.MaxDeelnemers;
                            existingLes.IsActief = apiLes.IsActief;
                            existingLes.LastSynced = DateTime.Now;

                            _context.Lessen.Update(existingLes);
                            Debug.WriteLine($"📝 Updated lesson: {apiLes.Naam}");
                        }
                    }

                    await _context.SaveChangesAsync();
                    Debug.WriteLine($"✅ Saved {lessenResponse.Data.Count} lessons to local database");
                }
                else
                {
                    Debug.WriteLine($"❌ Failed to sync lessons: {lessenResponse?.Message}");
                }

                // 2. Synchroniseer inschrijvingen als gebruiker ingelogd is
                if (!string.IsNullOrEmpty(General.UserId))
                {
                    var inschrijvingenResponse = await _apiService
                        .GetUserInschrijvingenAsync(General.UserId);

                    if (inschrijvingenResponse.Success && inschrijvingenResponse.Data != null)
                    {
                        Debug.WriteLine($"📋 Received {inschrijvingenResponse.Data.Count} registrations from API");

                        var oldInschrijvingen = await _context.Inschrijvingen
                            .Where(i => i.GebruikerId == General.UserId)
                            .ToListAsync();

                        if (oldInschrijvingen.Count > 0)
                        {
                            _context.Inschrijvingen.RemoveRange(oldInschrijvingen);  // Verwijder oude inschrijvingen
                            Debug.WriteLine($"🗑️ Removed {oldInschrijvingen.Count} old registrations");
                        }

                        foreach (var inschrijving in inschrijvingenResponse.Data)
                        {
                            _context.Inschrijvingen.Add(inschrijving);  // Voeg nieuwe inschrijvingen toe
                        }

                        await _context.SaveChangesAsync();
                        Debug.WriteLine($"✅ Saved {inschrijvingenResponse.Data.Count} registrations to local database");
                    }
                }

                Debug.WriteLine("🎉 Synchronization completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ REAL API Sync error: {ex.Message}");
                throw;
            }
            finally
            {
                _isBusy = false;
            }
        }

        // Ruim oude data op
        public async Task CleanupOldData()
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-30);
                var oldLessen = await _context.Lessen
                    .Where(l => l.StartTijd < cutoffDate)  // Lessen ouder dan 30 dagen
                    .ToListAsync();

                if (oldLessen.Count > 0)
                {
                    _context.Lessen.RemoveRange(oldLessen);  // Verwijder oude lessen
                    await _context.SaveChangesAsync();
                    Debug.WriteLine($"🧹 Cleaned up {oldLessen.Count} old lessons");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cleaning up old data: {ex.Message}");
            }
        }

        // Loguit gebruiker
        public void Logout()
        {
            General.ClearUserInfo();
            _apiService.SetToken(null);
            Debug.WriteLine("👋 User logged out");
        }
    }
}