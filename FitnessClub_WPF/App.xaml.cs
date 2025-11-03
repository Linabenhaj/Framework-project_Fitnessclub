using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FitnessClub.Models.Models;
using FitnessClub.Models.Data;
using System.Threading.Tasks;

namespace FitnessClub.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Database en seed data
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FitnessClubDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Gebruiker>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                context.Database.EnsureCreated();
                await SeedDataAsync(userManager, roleManager, context);
            }

            base.OnStartup(e);
        }

        private async Task SeedDataAsync(UserManager<Gebruiker> userManager, RoleManager<IdentityRole> roleManager, FitnessClubDbContext context)
        {
            // Rollen aanmaken
            string[] roleNames = { "Admin", "Lid", "Trainer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin gebruiker aanmaken
            var adminUser = await userManager.FindByEmailAsync("admin@fitness.com");
            if (adminUser == null)
            {
                adminUser = new Gebruiker
                {
                    Voornaam = "Admin",
                    Achternaam = "User",
                    UserName = "admin@fitness.com",
                    Email = "admin@fitness.com",
                    Geboortedatum = new DateTime(1980, 1, 1)
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Test data voor leden
            if (!context.Leden.Any())
            {
                context.Leden.AddRange(
                    new Lid { Voornaam = "Jan", Achternaam = "Jansen", Email = "jan@example.com", Telefoon = "0612345678", Geboortedatum = new DateTime(1990, 5, 15) },
                    new Lid { Voornaam = "Marie", Achternaam = "Pieters", Email = "marie@example.com", Telefoon = "0687654321", Geboortedatum = new DateTime(1985, 8, 22) }
                );
                context.SaveChanges();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<FitnessClubDbContext>();

            // Identity
            services.AddIdentity<Gebruiker, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<FitnessClubDbContext>()
            .AddDefaultTokenProviders();

            // Windows
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegisterWindow>();
            services.AddTransient<DashboardWindow>();
        }
    }
}