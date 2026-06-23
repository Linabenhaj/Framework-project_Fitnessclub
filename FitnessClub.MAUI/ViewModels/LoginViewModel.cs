using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using FitnessClub.MAUI.Views;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor de loginpagina
    public partial class LoginViewModel : BaseViewModel
    {
        // service voor API calls
        private readonly ApiService _apiService;

        // tweerichtings binding met de Entry velden in XAML
        [ObservableProperty] private string email = "admin@fitnessclub.be";
        [ObservableProperty] private string password = "Admin123!";
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private bool showError;

        // ApiService wordt via Dependency Injection meegegeven
        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Inloggen";
        }

        // command die in XAML wordt opgeroepen door de Aanmelden knop
        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;
            IsBusy = true;
            ShowError = false;
            ErrorMessage = "";

            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Vul zowel e-mail als wachtwoord in";
                    ShowError = true;
                    return;
                }

                // oude lokale data wissen vóór nieuwe login
                General.ClearUserInfo();

                // API call naar account login
                var result = await _apiService.LoginAsync(Email, Password);

                if (result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    // rol uit het JWT token halen
                    var rol = result.Roles?.FirstOrDefault() ?? "Lid";

                    // user info en token opslaan in Preferences voor auto re-login
                    General.SaveUserInfo(
                        userId:    result.Id    ?? Guid.NewGuid().ToString(),
                        email:     result.Email ?? Email,
                        firstName: result.Voornaam ?? "",
                        lastName:  result.Achternaam ?? "",
                        role:      rol,
                        token:     result.Token);

                    // token doorgeven aan HttpClient voor alle volgende calls
                    _apiService.SetToken(result.Token);

                    // navigatie afhankelijk van rol
                    if (rol.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                        await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
                    else
                        await Shell.Current.GoToAsync(nameof(DashboardPage));
                }
                else
                {
                    ErrorMessage = result.Message ?? "Login mislukt. Controleer je gegevens.";
                    ShowError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Verbindingsfout: {ex.Message}";
                ShowError = true;
                Debug.WriteLine($"Login error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // navigatie naar registratie pagina
        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        // terug naar home pagina
        [RelayCommand]
        private async Task NavigateToHome()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
