using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class AboutViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string appVersion = "1.0.0";

        [ObservableProperty]
        private string appName = "FitnessClub MAUI";

        [ObservableProperty]
        private string developer = "FitnessClub Development Team";

        [ObservableProperty]
        private string copyright = "© 2024 FitnessClub. Alle rechten voorbehouden.";

        [ObservableProperty]
        private string description = "De officiële FitnessClub mobiele applicatie voor het beheren van lessen, inschrijvingen en persoonlijke fitness doelen.";

        public AboutViewModel()
        {
            Title = "Over deze App";
            LoadVersionInfo();
        }

        private void LoadVersionInfo()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                AppVersion = version?.ToString() ?? "1.0.0";
                AppName = AppInfo.Current.Name;
            }
            catch
            {
                AppVersion = "1.0.0";
                AppName = "FitnessClub MAUI";
            }
        }

        [RelayCommand]
        private async Task OpenWebsite()
        {
            try
            {
                var url = "https://www.fitnessclub.example.com";
                await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout",
                    $"Kon website niet openen: {ex.Message}", "OK");
            }
        }

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

                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout",
                    $"Kon email niet openen: {ex.Message}", "OK");
            }
        }
    }
}