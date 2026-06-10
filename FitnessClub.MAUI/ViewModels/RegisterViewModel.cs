using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using System.Collections.ObjectModel;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor de registratiepagina
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] private string voornaam = string.Empty;
        [ObservableProperty] private string achternaam = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string telefoon = string.Empty;
        [ObservableProperty] private string wachtwoord = string.Empty;
        [ObservableProperty] private string bevestigWachtwoord = string.Empty;
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private bool showError;

        public ObservableCollection<string> Abonnementen { get; } = new ObservableCollection<string>
        {
            "Basic - €19.99/maand",
            "Medium - €34.99/maand",
            "Pro - €54.99/maand"
        };

        [ObservableProperty] private string? geselecteerdAbonnement;

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Registreren";
        }

        [RelayCommand]
        private async Task Register()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Voornaam) || string.IsNullOrWhiteSpace(Achternaam) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Wachtwoord))
            {
                ErrorMessage = "Vul alle verplichte velden in";
                ShowError = true;
                return;
            }

            if (Wachtwoord != BevestigWachtwoord)
            {
                ErrorMessage = "Wachtwoorden komen niet overeen";
                ShowError = true;
                return;
            }

            if (Wachtwoord.Length < 6)
            {
                ErrorMessage = "Wachtwoord moet minstens 6 tekens bevatten";
                ShowError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(GeselecteerdAbonnement))
            {
                ErrorMessage = "Kies een abonnement";
                ShowError = true;
                return;
            }

            IsBusy = true;
            ShowError = false;

            try
            {
                var dto = new RegistratieDto
                {
                    Email = Email,
                    Wachtwoord = Wachtwoord,
                    Voornaam = Voornaam,
                    Achternaam = Achternaam,
                    Telefoon = Telefoon
                };

                var result = await _apiService.RegisterAsync(dto);

                if (result.Success)
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Gelukt",
                        $"Je account is aangemaakt met abonnement '{GeselecteerdAbonnement}'. Je kunt nu inloggen.",
                        "OK");
                    await Shell.Current.GoToAsync("LoginPage");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Registratie mislukt. Probeer opnieuw.";
                    ShowError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Fout: {ex.Message}";
                ShowError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task BackToLogin()
        {
            await Shell.Current.GoToAsync("LoginPage");
        }
    }
}
