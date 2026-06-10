using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using System.Collections.ObjectModel;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor de gebruikerslijst van admin
    public partial class GebruikersViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] private ObservableCollection<GebruikerInfo> gebruikers = new();
        [ObservableProperty] private bool isRefreshing;

        public GebruikersViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Gebruikers";
            _ = LoadGebruikersAsync();
        }

        public async Task LoadGebruikersAsync()
        {
            IsBusy = true;
            Gebruikers.Clear();

            try
            {
                var result = await _apiService.GetAllUsersAsync();
                if (result.Success && result.Data != null)
                {
                    foreach (var g in result.Data)
                        Gebruikers.Add(g);
                }
                else
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                        result.Message ?? "Kon gebruikers niet ophalen", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout", $"Fout: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadGebruikersAsync();
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task VerwijderGebruiker(GebruikerInfo gebruiker)
        {
            if (gebruiker == null || string.IsNullOrEmpty(gebruiker.Id)) return;

            if (gebruiker.Id == General.UserId)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                    "Niet toegestaan", "Je kan je eigen account niet verwijderen.", "OK");
                return;
            }

            if (gebruiker.Rol?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                    "Niet toegestaan", "Een admin-account kan niet verwijderd worden.", "OK");
                return;
            }
            if (gebruiker.Rol?.Equals("Trainer", StringComparison.OrdinalIgnoreCase) == true)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                    "Niet toegestaan", "Een trainer-account kan niet verwijderd worden.", "OK");
                return;
            }

            bool confirm = await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                "Verwijderen",
                $"Wil je '{gebruiker.Voornaam} {gebruiker.Achternaam}' echt verwijderen?",
                "Ja", "Nee");

            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _apiService.DeleteUserAsync(gebruiker.Id);
                if (result.Success)
                {
                    Gebruikers.Remove(gebruiker);
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Verwijderd", $"'{gebruiker.Voornaam}' is verwijderd.", "OK");
                }
                else
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                        result.Message ?? "Verwijderen mislukt", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
