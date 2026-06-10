using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor de profielpagina
    public partial class ProfielViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] private string voornaam = "";
        [ObservableProperty] private string achternaam = "";
        [ObservableProperty] private string email = "";
        [ObservableProperty] private string telefoon = "Niet ingevuld";
        [ObservableProperty] private string rol = "";
        [ObservableProperty] private string abonnementNaam = "Geen abonnement";
        [ObservableProperty] private string abonnementPrijs = "";
        [ObservableProperty] private int aantalInschrijvingen = 0;

        public string ContactEmail => "klantendienst@fitnessclub.be";
        public string ContactTelefoon => "+32 3 123 45 67";
        public bool MagKlantendienstZien => General.IsLid;

        public ProfielViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Mijn Profiel";
            LoadProfile();
        }

        private void LoadProfile()
        {
            Voornaam = string.IsNullOrEmpty(General.UserFirstName) ? "Gebruiker" : General.UserFirstName;
            Achternaam = General.UserLastName ?? "";
            Email = General.UserEmail ?? "";
            Rol = General.UserRole ?? "Lid";

            _ = LoadExtraInfoAsync();
        }

        private async Task LoadExtraInfoAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(General.UserId))
                {
                    var insResult = await _apiService.GetUserInschrijvingenAsync(General.UserId);
                    if (insResult.Success && insResult.Data != null)
                    {
                        AantalInschrijvingen = insResult.Data.Count(i => i.Status == "Actief");
                    }
                }

                if (Rol.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    AbonnementNaam = "Beheerder (geen abonnement nodig)";
                }
                else if (Rol.Equals("Trainer", StringComparison.OrdinalIgnoreCase))
                {
                    AbonnementNaam = "Trainer-account";
                }
                else
                {
                    AbonnementNaam = "Basic";
                    AbonnementPrijs = "€19.99/maand";
                }
            }
            catch { }
        }

        [RelayCommand]
        private async Task ContactKlantendienst()
        {
            await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                "Klantendienst",
                $"Voor het wijzigen van persoonlijke gegevens of abonnement:\n\n" +
                $"📧 {ContactEmail}\n" +
                $"📞 {ContactTelefoon}\n\n" +
                $"Onze klantendienst is bereikbaar van maandag tot vrijdag, 9u-18u.",
                "OK");
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsBusy = true;
            LoadProfile();
            await Task.Delay(300);
            IsBusy = false;
        }
    }
}
