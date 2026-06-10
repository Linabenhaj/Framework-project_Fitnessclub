using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using FitnessClub.MAUI.Views;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor het admin dashboard
    public partial class AdminDashboardViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] private int totaalGebruikers;
        [ObservableProperty] private int totaalLessen;
        [ObservableProperty] private string adminNaam = "Admin";

        public AdminDashboardViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Admin Dashboard";
            AdminNaam = string.IsNullOrEmpty(General.UserFirstName) ? "Admin" : General.UserFirstName;
            _ = LoadStatsAsync();
        }

        public async Task LoadStatsAsync()
        {
            IsBusy = true;
            try
            {
                await Task.Delay(150);

                var lessenResult = await _apiService.GetAllLessenAsync();
                if (lessenResult.Success && lessenResult.Data != null)
                    TotaalLessen = lessenResult.Data.Count(l => l.IsActief);
                else
                    TotaalLessen = 0;

                var usersResult = await _apiService.GetAllUsersAsync();
                if (usersResult.Success && usersResult.Data != null)
                    TotaalGebruikers = usersResult.Data.Count;
                else
                    TotaalGebruikers = 0;
            }
            catch { }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task BeheerLessen() => await Shell.Current.GoToAsync("//LessenShell");

        [RelayCommand]
        private async Task BeheerInschrijvingen() => await Shell.Current.GoToAsync("//InschrijvingenShell");

        [RelayCommand]
        private async Task BekijkGebruikers() => await Shell.Current.GoToAsync("//GebruikersShell");

        [RelayCommand]
        private async Task NieuweLes()
        {
            string naam = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Nieuwe les", "Naam van de les:", placeholder: "bijv. Yoga, Spinning...");
            if (string.IsNullOrWhiteSpace(naam)) return;

            string locatie = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Nieuwe les", "Locatie:", placeholder: "bijv. Zaal A");
            if (string.IsNullOrWhiteSpace(locatie)) return;

            string maxStr = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Nieuwe les", "Max. deelnemers:", keyboard: Microsoft.Maui.Keyboard.Numeric, placeholder: "20");
            int max = int.TryParse(maxStr, out var m) ? m : 20;

            IsBusy = true;
            try
            {
                var result = await _apiService.CreateLesAsync(new NewLesDto
                {
                    Naam = naam,
                    Trainer = "",
                    Locatie = locatie,
                    MaxDeelnemers = max,
                    StartTijd = DateTime.Now.AddDays(7),
                    EindTijd = DateTime.Now.AddDays(7).AddHours(1),
                    Beschrijving = "",
                    IsActief = true
                });

                if (result.Success)
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Aangemaakt",
                        $"Les '{naam}' is aangemaakt!\nEen trainer kan zich nu opgeven via de Lessen-pagina.",
                        "OK");
                    await LoadStatsAsync();
                }
                else
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Fout", result.Message ?? "Aanmaken mislukt", "OK");
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task Refresh() => await LoadStatsAsync();

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                "Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");

            if (!confirm) return;

            General.ClearUserInfo();
            _apiService.SetToken(null);
            AppShell.Instance?.SetFlyoutVisible(false);
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}
