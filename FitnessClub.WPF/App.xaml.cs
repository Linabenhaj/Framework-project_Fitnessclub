using FitnesclubLedenbeheer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class App : Application
    {
        private IHost _host;
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Database
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDB;Trusted_Connection=true;MultipleActiveResultSets=true"));

                    // Windows
                    services.AddTransient<MainWindow>();
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<RegisterWindow>();
                    services.AddTransient<LoginFormWindow>();

                    // Views
                    services.AddTransient<Views.LedenOverzicht>();
                    services.AddTransient<Views.LidToevoegen>();
                    services.AddTransient<Views.AbonnementenOverzicht>();
                    services.AddTransient<Views.AbonnementToevoegen>();
                    services.AddTransient<Views.InschrijvingenOverzicht>();
                    services.AddTransient<Views.InschrijvingToevoegen>();
                    services.AddTransient<Views.BetalingenOverzicht>();
                    services.AddTransient<Views.BetalingToevoegen>();
                })
                .Build();

            ServiceProvider = _host.Services;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Create database
            using (var scope = _host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await context.Database.EnsureCreatedAsync();
            }

            // Show login window
            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }
    }
}