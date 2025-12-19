using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel  // ViewModel voor login pagina
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string email = "admin@fitness.com";  // Standaard demo email

        [ObservableProperty]
        private string password = "admin123";  // Standaard demo wachtwoord

        [ObservableProperty]
        private string errorMessage = string.Empty;  // Foutmelding bij login

        [ObservableProperty]
        private bool showError;  // Toon foutmelding

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Inloggen";

            _ = TestConnectionOnStartup();  // Test API verbinding bij opstart
        }

        // Test API verbinding bij app opstart
        private async Task TestConnectionOnStartup()
        {
            try
            {
                Debug.WriteLine($"🔗 Startup: Testing API connection...");
                Debug.WriteLine($"🔗 API Base URL: {_apiService.GetBaseUrl()}");

                var result = await _apiService.TestConnectionAsync();
                Debug.WriteLine($"🔗 Startup connection test: {(result ? "✅ OK" : "❌ FAILED")}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🔗 Startup test error: {ex.Message}");
            }
        }

        // Login gebruiker via API
        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;

            IsBusy = true;
            ShowError = false;
            ErrorMessage = "";

            try
            {
                // Valideer input velden
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Vul zowel e-mail als wachtwoord in";
                    ShowError = true;
                    return;
                }

                Debug.WriteLine($"🔐 Login attempt for: {Email}");
                Debug.WriteLine($"🔐 Using API URL: {_apiService.GetBaseUrl()}");

                var result = await _apiService.LoginAsync(Email, Password);  // API login call

                Debug.WriteLine($"🔐 API Response - Success: {result.Success}");
                Debug.WriteLine($"🔐 API Response - Token: {(string.IsNullOrEmpty(result.Token) ? "MISSING" : "PRESENT")}");
                Debug.WriteLine($"🔐 API Response - Message: {result.Message}");

                if (result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    Debug.WriteLine($"✅ Login successful!");

                    // Sla gebruiker info op in Preferences
                    General.SaveUserInfo(
                        userId: result.Id ?? Guid.NewGuid().ToString(),
                        email: result.Email ?? Email,
                        firstName: result.Voornaam ?? Email.Split('@')[0],
                        lastName: result.Achternaam ?? "",
                        role: result.Roles?.FirstOrDefault() ?? "Gebruiker",
                        token: result.Token
                    );

                    _apiService.SetToken(result.Token);  // Stel token in voor toekomstige requests

                    Debug.WriteLine($"👤 User saved: {General.UserFirstName} {General.UserLastName}");
                    Debug.WriteLine($"🎭 Role: {General.UserRole}");

                    await Shell.Current.GoToAsync("//DashboardPage");  // Navigeer naar dashboard
                }
                else
                {
                    ErrorMessage = result.Message ?? "Login mislukt. Controleer je inloggegevens.";
                    ShowError = true;
                    Debug.WriteLine($"❌ Login failed: {result.Message}");

                    // Toon debug info voor ontwikkelaar
                    await Application.Current.MainPage.DisplayAlert("Debug Info",
                        $"Login Response:\n\n" +
                        $"Success: {result.Success}\n" +
                        $"Message: {result.Message}\n" +
                        $"Token: {(string.IsNullOrEmpty(result.Token) ? "Geen" : "Ja")}\n" +
                        $"API URL: {_apiService.GetBaseUrl()}",
                        "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = $"Netwerkfout: {httpEx.Message}";
                ShowError = true;
                Debug.WriteLine($"🌐 Network error: {httpEx.Message}");

                await Application.Current.MainPage.DisplayAlert("Netwerk Fout",
                    $"Kan geen verbinding maken:\n\n{httpEx.Message}\n\n" +
                    $"Probeer:\n1. Firewall uitzetten\n2. 'Test Direct Login' knop gebruiken",
                    "OK");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Fout: {ex.Message}";
                ShowError = true;
                Debug.WriteLine($"💥 Exception: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Test API verbinding via service
        [RelayCommand]
        private async Task TestConnection()
        {
            try
            {
                IsBusy = true;
                Debug.WriteLine("🔗 Testing API connection via service...");

                var result = await _apiService.TestConnectionAsync();

                if (result)
                {
                    await Application.Current.MainPage.DisplayAlert("✅ Verbinding OK",
                        $"De app kan verbinding maken met de API!\n\nURL: {_apiService.GetBaseUrl()}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("❌ Geen verbinding",
                        $"Kan geen verbinding maken met de API.\n\nURL: {_apiService.GetBaseUrl()}\n\n" +
                        $"Gebruik 'Test Direct Login' knop om te debuggen.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("❌ Fout",
                    $"Test mislukt: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Toon demo accounts informatie
        [RelayCommand]
        private async Task ShowDemoAccounts()
        {
            var message = "📋 Test Accounts\n\n" +
                         "admin@fitness.com / admin123\n" +
                         "user@fitness.com / user123\n\n" +
                         "ℹ️ Deze accounts werken met de test API";

            await Application.Current.MainPage.DisplayAlert("Test Accounts", message, "OK");
        }

        // Navigeer naar registratie pagina
        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        // Navigeer als gast (zonder login)
        [RelayCommand]
        private async Task NavigateAsGuest()
        {
            await Shell.Current.GoToAsync("//LessenPage");
        }
    }
}