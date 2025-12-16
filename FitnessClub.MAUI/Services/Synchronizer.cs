using System.Diagnostics;
using System.Text.Json;
using FitnessClub.MAUI.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.MAUI.Services
{
    public class Synchronizer
    {
        private readonly HttpClient _httpClient;
        private readonly LocalDbContext _context;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ApiService _apiService;

        public bool DatabaseExists { get; private set; }

        public Synchronizer(LocalDbContext context, ApiService apiService)
        {
            _context = context;
            _apiService = apiService;

            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };

            _httpClient.BaseAddress = new Uri(General.ApiUrl);
        }

        public async Task InitializeDatabase()
        {
            try
            {
                await _context.Database.MigrateAsync();
                await SeedDefaultData();
                DatabaseExists = true;
                Debug.WriteLine("Database initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }

        private async Task SeedDefaultData()
        {
            if (!await _context.Users.AnyAsync())
            {
                var localUser = new LocalUser
                {
                    Id = "local-user",
                    UserName = "localuser",
                    Email = "local@fitness.com",
                    Voornaam = "Local",
                    Achternaam = "User",
                    Geboortedatum = DateTime.Now.AddYears(-30),
                    PhoneNumber = "",
                    EmailConfirmed = true
                };

                _context.Users.Add(localUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> Login(string email, string password)
        {
            try
            {
                // DEMO: always succeed and store demo user info
                var userId = Guid.NewGuid().ToString();
                var displayName = email.Split('@')[0];

                await General.SaveUserInfo(
                    userId,           // userId
                    email,            // email
                    displayName,      // name
                    "Gebruiker",      // role
                    "demo-jwt-token", // token
                    "demo-token-123"  // extra token parameter
                );

                await SynchronizeAll();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsAuthorized()
        {
            await General.LoadUserInfo();
            return !string.IsNullOrEmpty(General.Token);
        }

        public async Task SynchronizeAll()
        {
            try
            {
                Debug.WriteLine("Starting synchronization...");
                // Demo synchronization
                await Task.Delay(500);
                Debug.WriteLine("Synchronization completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Synchronization error: {ex.Message}");
            }
        }

        public void Logout()
        {
            General.ClearUserInfo();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
         public async Task CleanupOldData()
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-30);
                var oldLessen = await _context.Lessen
                    .Where(l => l.StartTijd < cutoffDate)
                    .ToListAsync();

                if (oldLessen.Any())
                {
                    _context.Lessen.RemoveRange(oldLessen);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cleaning up old data: {ex.Message}");
            }
        }
    }
}