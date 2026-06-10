using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.Models.Models;
using FitnessClub.MAUI.Services;
using System.Collections.ObjectModel;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor de lessenpagina
    public partial class LessenViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] private ObservableCollection<LocalLes> lessen = new();
        [ObservableProperty] private string searchText = "";
        [ObservableProperty] private bool showOnlyAvailable = true;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool isOffline;

        public bool MagInschrijven => !General.IsAdmin && !General.IsTrainer;
        public bool MagBewerken => General.IsAdmin;
        public bool MagClaimen => General.IsTrainer;
        public bool MagVerwijderen => General.IsAdmin;

        private List<LocalLes> _allLessen = new();
        private HashSet<int> _ingeschrevenLesIds = new();

        public LessenViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Lessen";
            _ = LoadLessenAsync();
        }

        public async Task LoadLessenAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Lessen.Clear();

            try
            {
                var result = await _apiService.GetAllLessenAsync();
                if (result.Success && result.Data != null)
                {
                    _allLessen = result.Data;
                    IsOffline = false;

                    if (!string.IsNullOrEmpty(General.UserId) && !General.IsAdmin)
                    {
                        var insResult = await _apiService.GetUserInschrijvingenAsync(General.UserId);
                        if (insResult.Success && insResult.Data != null)
                        {
                            _ingeschrevenLesIds = insResult.Data
                                .Where(i => i.Status == "Actief")
                                .Select(i => i.LesId)
                                .ToHashSet();

                            foreach (var les in _allLessen)
                                les.IsIngeschrevenDoorMij = _ingeschrevenLesIds.Contains(les.Id);
                        }
                    }

                    ApplyFilter();
                }
                else
                {
                    IsOffline = true;
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                        result.Message ?? "Kon lessen niet ophalen.", "OK");
                }
            }
            catch (Exception ex)
            {
                IsOffline = true;
                await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                    $"Onverwachte fout: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilter()
        {
            Lessen.Clear();
            var filtered = _allLessen
                .Where(l => l.IsActief && l.StartTijd > DateTime.Now)
                .AsEnumerable();

            if (ShowOnlyAvailable)
                filtered = filtered.Where(l => l.AantalIngeschreven < l.MaxDeelnemers);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                filtered = filtered.Where(l =>
                    (l.Naam ?? "").ToLower().Contains(s) ||
                    (l.Trainer ?? "").ToLower().Contains(s) ||
                    (l.Locatie ?? "").ToLower().Contains(s));
            }

            foreach (var les in filtered.OrderBy(l => l.StartTijd))
                Lessen.Add(les);
        }

        [RelayCommand]
        private void Search() => ApplyFilter();

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadLessenAsync();
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task Inschrijven(LocalLes les)
        {
            if (les == null) return;

            if (les.IsIngeschrevenDoorMij)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                    "Reeds ingeschreven",
                    $"Je bent al ingeschreven voor '{les.Naam}'.\n\nOm uit te schrijven, ga naar 'Mijn Inschrijvingen'.",
                    "OK");
                return;
            }

            bool confirm = await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                "Inschrijven",
                $"Inschrijven voor '{les.Naam}'?\nDatum: {les.StartTijd:dd/MM/yyyy HH:mm}\nTrainer: {les.Trainer}",
                "Ja", "Nee");

            if (!confirm) return;

            if (string.IsNullOrEmpty(General.UserId))
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                    "Je moet ingelogd zijn om in te schrijven", "OK");
                return;
            }

            try
            {
                var result = await _apiService.CreateInschrijvingAsync(new InschrijvingDto
                {
                    GebruikerId = General.UserId,
                    LesId = les.Id,
                    InschrijfDatum = DateTime.Now,
                    Status = "Actief"
                });

                if (result.Success)
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Succes", $"Je bent ingeschreven voor '{les.Naam}'!", "OK");
                    await LoadLessenAsync();
                }
                else
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                        result.Message ?? "Inschrijven mislukt", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                    $"Inschrijven mislukt: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task BewerkLes(LocalLes les)
        {
            if (les == null) return;

            string nieuweNaam = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Les bewerken", "Naam:", initialValue: les.Naam);
            if (string.IsNullOrWhiteSpace(nieuweNaam)) return;

            string nieuweTrainer = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Les bewerken", "Trainer:", initialValue: les.Trainer);
            if (nieuweTrainer == null) return;

            string nieuweLocatie = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Les bewerken", "Locatie:", initialValue: les.Locatie);
            if (nieuweLocatie == null) return;

            string maxStr = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Les bewerken", "Max. deelnemers:",
                keyboard: Microsoft.Maui.Keyboard.Numeric,
                initialValue: les.MaxDeelnemers.ToString());
            int max = int.TryParse(maxStr, out var m) ? m : les.MaxDeelnemers;

            les.Naam = nieuweNaam;
            les.Trainer = nieuweTrainer;
            les.Locatie = nieuweLocatie;
            les.MaxDeelnemers = max;

            try
            {
                var result = await _apiService.UpdateLesAsync(les);
                if (result.Success)
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Bijgewerkt", $"Les '{nieuweNaam}' is bijgewerkt!", "OK");
                    await LoadLessenAsync();
                }
                else
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                        result.Message ?? "Bewerken mislukt", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                    $"Bewerken mislukt: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task VerwijderLes(LocalLes les)
        {
            if (les == null) return;

            bool confirm = await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                "Verwijderen",
                $"Wil je '{les.Naam}' echt verwijderen?",
                "Ja", "Nee");

            if (!confirm) return;

            try
            {
                var result = await _apiService.DeleteLesAsync(les.Id);
                if (result.Success)
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Verwijderd", $"Les '{les.Naam}' is verwijderd.", "OK");
                    await LoadLessenAsync();
                }
                else
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                        result.Message ?? "Verwijderen mislukt", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                    $"Verwijderen mislukt: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ClaimLes(LocalLes les)
        {
            if (les == null) return;

            string trainerNaam = await Application.Current!.Windows[0]!.Page!.DisplayPromptAsync(
                "Claim les", "Jouw naam als trainer:",
                initialValue: $"{General.UserFirstName} {General.UserLastName}".Trim());
            if (string.IsNullOrWhiteSpace(trainerNaam)) return;

            les.Trainer = trainerNaam;
            try
            {
                var result = await _apiService.UpdateLesAsync(les);
                if (result.Success)
                {
                    await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                        "Geclaimd", $"Je bent nu de trainer van '{les.Naam}'!", "OK");
                    await LoadLessenAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0]!.Page!.DisplayAlert("Fout",
                    $"Claimen mislukt: {ex.Message}", "OK");
            }
        }
    }
}
