using FitnessClub.MAUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class AboutViewModel : BaseViewModel  // ViewModel voor "Over" pagina
    {
        [ObservableProperty]
        private string appVersion = "1.0.0";  // Huidige app versie

        [ObservableProperty]
        private string appName = "FitnessClub MAUI";  // App naam

        [ObservableProperty]
        private string developer = "FitnessClub Development Team";  // Ontwikkelaar

        [ObservableProperty]
        private string copyright = "© 2024 FitnessClub. Alle rechten voorbehouden.";  // Copyright

        [ObservableProperty]
        private string description = "De officiële FitnessClub mobiele applicatie voor het beheren van lessen, inschrijvingen en persoonlijke fitness doelen.";  // App beschrijving

        public AboutViewModel()
        {
            Title = "Over deze App";
            LoadVersionInfo();  // Laad versie informatie
        }

        // Laad app versie informatie
        private void LoadVersionInfo()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;  // Haal assembly versie op
                AppVersion = version?.ToString() ?? "1.0.0";
                AppName = AppInfo.Current.Name;  // Haal app naam op
            }
            catch
            {
                AppVersion = "1.0.0";
                AppName = "FitnessClub MAUI";
            }
        }

        // Open website in browser
        [RelayCommand]
        private async Task OpenWebsite()
        {
            try
            {
                var url = "https://www.fitnessclub.example.com";
                await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);  // Open browser
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout",
                    $"Kon website niet openen: {ex.Message}", "OK");
            }
        }

        // Open email client voor feedback
        [RelayCommand]
        private async Task SendFeedback()
        {
            try
            {
                var email = "support@fitnessclub.example.com";
                var subject = "Feedback FitnessClub App";
                var body = $"App Versie: {AppVersion}\n\nMijn feedback: ";

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = new List<string> { email }
                };

                await Email.Default.ComposeAsync(message);  // Open email client
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout",
                    $"Kon email niet openen: {ex.Message}", "OK");
            }
        }
    }
}