using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email = "admin@fitness.com"; // Pre-filled admin

        [ObservableProperty]
        private string password = "admin123";

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool showError;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Inloggen";
        }

        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;

            IsBusy = true;
            ShowError = false;

            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Vul zowel e-mail als wachtwoord in";
                    ShowError = true;
                    return;
                }

                var result = await _authService.LoginAsync(Email, Password);

                if (result.Success && result.User != null)
                {
                    Debug.WriteLine($"Ingelogd als: {result.User.Role} - {result.User.Name}");

                    // Navigeer naar juiste dashboard op basis van rol
                    switch (result.User.Role.ToLower())
                    {
                        case "admin":
                            await Shell.Current.GoToAsync("//AdminDashboardPage");
                            break;
                        case "trainer":
                            // Voor trainer: ga naar HomePage (kan later aangepast worden)
                            await Shell.Current.GoToAsync("//HomePage");
                            break;
                        default:
                            await Shell.Current.GoToAsync("//HomePage");
                            break;
                    }
                }
                else
                {
                    ErrorMessage = result.Message;
                    ShowError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden: {ex.Message}";
                ShowError = true;
                Debug.WriteLine($"Login exception: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ShowDemoAccounts()
        {
            var message = "Demo accounts:\n\n" +
                         "?? Admin: admin@fitness.com / admin123\n" +
                         "?? Trainer: trainer@fitness.com / trainer123\n" +
                         "?? Gebruiker: gebruiker@fitness.com / gebruiker123";

            await Application.Current.MainPage.DisplayAlert("Demo Accounts", message, "OK");
        }
    }
}