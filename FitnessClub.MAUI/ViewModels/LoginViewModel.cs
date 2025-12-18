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
        private string email = "admin@fitness.com";

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

                // AuthService
                if (result.Success)
                {
                    //  Email als userId als er geen Id is
                    string userId = Email;
                    string userName = Email.Split('@')[0];
                    string role = "Gebruiker"; // Default rol
                    string token = ""; // Zet leeg als er geen token is

                    await General.SaveUserInfo(
                        userId: userId,
                        email: Email,
                        firstName: userName,
                        lastName: "",
                        role: role,
                        token: token
                    );

                    // Navigeer naar HomePage
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Inloggen mislukt";
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
                         "👑 Admin: admin@fitness.com / admin123\n" +
                         "💪 Trainer: trainer@fitness.com / trainer123\n" +
                         "👤 Gebruiker: gebruiker@fitness.com / gebruiker123";

            await Application.Current.MainPage.DisplayAlert("Demo Accounts", message, "OK");
        }
    }
}