using Microsoft.Extensions.Logging;
using FitnessClub.MAUI.ViewModels;
using FitnessClub.MAUI.Views;
using FitnessClub.MAUI.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessClub.MAUI
{
    public static class MauiProgram  // Dependency Injection configuratie
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()  // Stel App als hoofdapplicatie in
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");  // Voeg lettertype toe
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");  // Voeg lettertype toe
                })
                .UseMauiCommunityToolkit();  // Voeg Community Toolkit toe

#if DEBUG
            builder.Logging.AddDebug();  // Debug logging in development mode
#endif

            // HttpClient configuratie met specifiek IP voor Android emulator
            builder.Services.AddHttpClient("FitnessApi", client =>
            {
                // Gebruik statisch IP voor API communicatie
                client.BaseAddress = new Uri("http://172.20.96.1:5000/api/");

                client.Timeout = TimeSpan.FromSeconds(15);  // Timeout voor requests

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));  // Accepteer JSON

                System.Diagnostics.Debug.WriteLine($"🌐 HttpClient configured with BaseAddress: {client.BaseAddress}");
            });

            // ApiService registratie met HttpClient factory
            builder.Services.AddSingleton<ApiService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("FitnessApi");
                return new ApiService(httpClient);
            });

            // Synchronizer registratie zonder LocalDbContext voor nu
            builder.Services.AddSingleton<Synchronizer>(sp =>
            {
                var apiService = sp.GetRequiredService<ApiService>();
                return new Synchronizer(null, apiService);  // null voor DbContext tijdelijk
            });

            // ViewModels registreren (Transient = nieuwe instantie per request)
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<LessenViewModel>();
            builder.Services.AddTransient<InschrijvingenViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<ProfielViewModel>();
            builder.Services.AddTransient<AboutViewModel>();

            // Pages registreren
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<LessenPage>();
            builder.Services.AddTransient<InschrijvingenPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ProfielPage>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AdminDashboardPage>();
            builder.Services.AddTransient<RegisterPage>();

            return builder.Build();  // Bouw en retourneer de MAUI app
        }
    }
}