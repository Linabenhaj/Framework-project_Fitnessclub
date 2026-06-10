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
        private readonly ApiService _apiService;

        [ObservableProperty] private string email = "admin@fitnessclub.be";
        [ObservableProperty] private string password = "Admin123!";
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private bool showError;

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Inloggen";
        }

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

                General.ClearUserInfo();

                var result = await _apiService.LoginAsync(Email, Password);

                if (result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    var rol = result.Roles?.FirstOrDefault() ?? "Lid";

                    General.SaveUserInfo(
                        userId:    result.Id    ?? Guid.NewGuid().ToString(),
                        email:     result.Email ?? Email,
                        firstName: result.Voornaam ?? "",
                        lastName:  result.Achternaam ?? "",
                        role:      rol,
                        token:     result.Token);

                    _apiService.SetToken(result.Token);

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

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        [RelayCommand]
        private async Task NavigateToHome()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}