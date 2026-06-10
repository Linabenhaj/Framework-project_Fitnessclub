using FitnessClub.MAUI.Services;
using System.Diagnostics;

namespace FitnessClub.MAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _apiService;

        public LoginPage(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasBackButton(this, false);

#if DEBUG
            DevPanel.IsVisible = true;
#endif
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            BusyIndicator.IsVisible = true;
            BusyIndicator.IsRunning = true;
            LoginButton.IsEnabled = false;
            ErrorBorder.IsVisible = false;

            try
            {
                var email = EmailEntry.Text?.Trim() ?? string.Empty;
                var password = PasswordEntry.Text ?? string.Empty;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ToonFout("Vul uw e-mailadres en wachtwoord in.");
                    return;
                }

                await Task.Yield();

                var result = await _apiService.LoginAsync(email, password);

                if (result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    var rol = result.Roles?.FirstOrDefault() ?? "Lid";
                    Debug.WriteLine($"[Login] Succesvol: {email} – Rol: {rol}");

                    if (AppShell.Instance != null)
                        await AppShell.Instance.OnLoginSucceeded(
                            userId:    result.Id ?? Guid.NewGuid().ToString(),
                            email:     result.Email ?? email,
                            firstName: result.Voornaam ?? "",
                            lastName:  result.Achternaam ?? "",
                            role:      rol,
                            token:     result.Token);
                }
                else
                {
                    ToonFout(result.Message ?? "Login mislukt. Controleer uw gegevens.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Login] Fout: {ex.Message}");
                ToonFout($"Verbindingsfout: controleer of de API actief is.\n({ex.Message})");
            }
            finally
            {
                BusyIndicator.IsRunning = false;
                BusyIndicator.IsVisible = false;
                LoginButton.IsEnabled = true;
            }
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        private void OnAutofillAdminClicked(object sender, EventArgs e)
        {
            EmailEntry.Text = "admin@fitnessclub.be";
            PasswordEntry.Text = "Admin123!";
        }

        private void OnAutofillLidClicked(object sender, EventArgs e)
        {
            EmailEntry.Text = "user@fitnessclub.be";
            PasswordEntry.Text = "User123!";
        }

        private void ToonFout(string bericht)
        {
            ErrorLabel.Text = bericht;
            ErrorBorder.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ErrorBorder.IsVisible = false;
            AppShell.Instance?.SetFlyoutVisible(false);
        }
    }
}
