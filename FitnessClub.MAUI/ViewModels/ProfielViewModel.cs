using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor de profielpagina
    public partial class ProfielViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        // properties met data van de ingelogde gebruiker
        [ObservableProperty] private string voornaam = "";
        [ObservableProperty] private string achternaam = "";
        [ObservableProperty] private string email = "";
        [ObservableProperty] private string telefoon = "Niet ingevuld";
        [ObservableProperty] private string rol = "";
        [ObservableProperty] private string abonnementNaam = "Geen abonnement";
        [ObservableProperty] private string abonnementPrijs = "";
        [ObservableProperty] private int aantalInschrijvingen = 0;

        // vaste contact info voor de klantendienst tekstbox
        public string ContactEmail => "klantendienst@fitnessclub.be";
        public string ContactTelefoon => "+32 3 123 45 67";

        // klantendienst tekstbox alleen tonen voor leden
        public bool MagKlantendienstZien => General.IsLid;

        public ProfielViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Mijn Profiel";
            LoadProfile();
        }

        // laadt voornaam achternaam email en rol uit General
        private void LoadProfile()
        {
            Voornaam = string.IsNullOrEmpty(General.UserFirstName) ? "Gebruiker" : General.UserFirstName;
            Achternaam = General.UserLastName ?? "";
            Email = General.UserEmail ?? "";
            Rol = General.UserRole ?? "Lid";

            _ = LoadExtraInfoAsync();
        }

        // haalt aantal inschrijvingen op via de API en zet de abonnement label op basis van rol
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
                    AbonnementNaam = "Beheerder (geen abonnement nodig)";
                else if (Rol.Equals("Trainer", StringComparison.OrdinalIgnoreCase))
                    AbonnementNaam = "Trainer-account";
                else
                {
                    AbonnementNaam = "Basic";
                    AbonnementPrijs = "€19.99/maand";
                }
            }
            catch { }
        }

        // herlaadt het profiel telkens de pagina opent
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
