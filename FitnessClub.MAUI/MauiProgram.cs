using CommunityToolkit.Maui;
using FitnessClub.MAUI.Services;
using FitnessClub.MAUI.ViewModels;
using FitnessClub.MAUI.Views;
using FitnessClub.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessClub.MAUI
{
    // startpunt van de MAUI app
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // initialiseer SQLite voor de app start
            SQLitePCL.Batteries_V2.Init();

            var builder = MauiApp.CreateBuilder();

            ConfigureMaui(builder);
            ConfigureServices(builder);
            ConfigureHttpClient(builder);
            ConfigureLogging(builder);

            var app = builder.Build();

            // auto re-login als er nog een geldig token bestaat
            if (General.IsLoggedIn && !string.IsNullOrEmpty(General.Token))
            {
                var api = app.Services.GetRequiredService<ApiService>();
                api.SetToken(General.Token);
            }

            StartBackgroundDbInit(app);

            return app;
        }

        private static void ConfigureMaui(MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
        }

        //  SERVICES 

        private static void ConfigureServices(MauiAppBuilder builder)
        {
            var services = builder.Services;

            // singleton voor login status
            services.AddSingleton<SecurityViewModel>();

            // SQLite database voor lokale opslag
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "fitnessclub.db");
            services.AddDbContext<LocalDbContext>(
                options => options.UseSqlite($"Data Source={dbPath}"), //Dbcontext wordt per request aangemaakt, dus Transient is hier correct
                ServiceLifetime.Transient);

            // service voor API calls!
            services.AddSingleton<ApiService>(sp =>
                new ApiService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("FitnessApi")));
            services.AddSingleton<AuthService>();

            //moeten in de hele app beschikbaar zijn
            // ViewModels per pagina
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<AdminDashboardViewModel>();
            services.AddTransient<LessenViewModel>();
            services.AddTransient<InschrijvingenViewModel>();
            services.AddTransient<ProfielViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<GebruikersViewModel>();

            // pagina registraties
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<LessenPage>();
            services.AddTransient<InschrijvingenPage>();
            services.AddTransient<AdminDashboardPage>();
            services.AddTransient<DashboardPage>();
            services.AddTransient<ProfielPage>();
            services.AddTransient<HomePage>();
            services.AddTransient<GebruikersPage>();

            //
            // hoofdshell van de app
            services.AddSingleton<AppShell>();
        }

        //  HTTP 

        private static void ConfigureHttpClient(MauiAppBuilder builder)
        {
            var apiBase = ResolveApiBase();// bepaalt de juiste API base URL afhankelijk van het platform (Android of andere)

            builder.Services.AddHttpClient("FitnessApi", client =>
            {
                client.BaseAddress = new Uri(apiBase);
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler // de handler doet de SSL certificaat validatie, hier wordt self-signed certificaten op localhost/
                {
                    ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
                    {
                        // In development: accepteer self-signed certificaten op localhost/10.0.2.2
#if DEBUG
                        var host = msg?.RequestUri?.Host ?? "";
                        if (host == "localhost" || host == "10.0.2.2") return true;
#endif
                        return errors == System.Net.Security.SslPolicyErrors.None;
                    }
                });
        }

        //  LOGGING 

        private static void ConfigureLogging(MauiAppBuilder builder)
        {
#if DEBUG
            builder.Logging.AddDebug();
#endif
        }

        //  DATABASE INIT 

        private static void StartBackgroundDbInit(MauiApp app)
        {
            // Fire-and-forget: blokkeer de app-start niet
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
                    await db.Database.EnsureCreatedAsync();
                    System.Diagnostics.Debug.WriteLine("[DB] SQLite tabellen aangemaakt of bevestigd.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[DB] Init fout (niet kritiek): {ex.Message}");
                }
            });
        }

     

        private static string ResolveApiBase()
        {
           
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "http://10.0.2.2:5000/api/";

            return "http://localhost:5000/api/";
        }
    }
}
