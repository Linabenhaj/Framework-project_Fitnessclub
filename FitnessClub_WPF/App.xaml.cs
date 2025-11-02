using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FitnessClub.Models.Models;
using FitnessClub.Models.Data;

namespace FitnessClub.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            //  database geüpdatet
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FitnessClubDbContext>();
                context.Database.EnsureCreated();
            }

            base.OnStartup(e);
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