using Microsoft.Extensions.Logging;
using FitnessClub.MAUI.ViewModels;
using FitnessClub.MAUI.Views;
using FitnessClub.MAUI.Models;
using FitnessClub.MAUI.Services;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Maui;
using Microsoft.Maui.Storage;

namespace FitnessClub.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Registreer DbContext
            builder.Services.AddDbContext<LocalDbContext>(options =>
                options.UseSqlite($"Data Source={Path.Combine(FileSystem.AppDataDirectory, "fitnessclub.db")}"));

            // Registreer Services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<Synchronizer>();

            // Registreer ViewModels
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<LessenViewModel>();
            builder.Services.AddTransient<InschrijvingenViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<ProfielViewModel>();
            builder.Services.AddTransient<AboutViewModel>();

            // Registreer Pages
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LessenPage>();
            builder.Services.AddTransient<InschrijvingenPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ProfielPage>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AdminDashboardPage>();
       
            builder.Services.AddTransient<RegisterPage>();
            return builder.Build();
        
            builder.Services.AddTransient<RegisterPage>();

        }
    }
}