using CommunityToolkit.Maui;
using FitnessClub.MAUI.ViewModels;
using FitnessClub.MAUI.Views;
using FitnessClub.MAUI.Views.Admin;

namespace FitnessClub.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // PAGES
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LessenPage>();
            builder.Services.AddTransient<ProfielPage>();
            builder.Services.AddTransient<InschrijvingenPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AdminDashboardPage>();

            // VIEWMODELS 
            builder.Services.AddTransient<ProfielViewModel>();

            return builder.Build();
        }
    }
}