using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Models;
using FitnessClub.MAUI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class LessenViewModel : BaseViewModel  // ViewModel voor lessen overzicht
    {
        private readonly LocalDbContext _context;
        private readonly Synchronizer _synchronizer;

        [ObservableProperty]
        private ObservableCollection<LocalLes> lessen = new();  // Lijst met lessen

        [ObservableProperty]
        private string searchText = "";  // Zoekterm voor filteren

        [ObservableProperty]
        private bool showOnlyAvailable = true;  // Toon alleen beschikbare lessen

        [ObservableProperty]
        private bool isRefreshing;  // Pull-to-refresh status

        public LessenViewModel(LocalDbContext context, Synchronizer synchronizer)
        {
            _context = context;
            _synchronizer = synchronizer;
            Title = "Lessen";
            LoadLessen();  // Laad lessen bij opstart
        }

        // Laad lessen uit database
        public async void LoadLessen()
        {
            if (IsBusy) return;
            IsBusy = true;
            Lessen.Clear();  // Maak lijst leeg

            try
            {
                var query = _context.Lessen
                    .Include(l => l.Inschrijvingen)  // Include inschrijvingen voor bezettingsgraad
                    .Where(l => l.IsActief && l.StartTijd > DateTime.Now);  // Filter actieve toekomstige lessen

                if (ShowOnlyAvailable)
                    query = query.Where(l => l.Inschrijvingen.Count(i => i.Status == "Actief") < l.MaxDeelnemers);  // Filter beschikbare lessen

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(l =>
                        l.Naam.Contains(SearchText) ||
                        l.Trainer.Contains(SearchText) ||
                        l.Locatie.Contains(SearchText));  // Zoek op naam, trainer of locatie
                }

                var lessonList = await query.OrderBy(l => l.StartTijd).ToListAsync();  // Sorteer op starttijd
                foreach (var les in lessonList)
                    Lessen.Add(les);  // Voeg toe aan observable collectie
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", $"Kon lessen niet laden: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Herlaad lessen bij zoekopdracht
        [RelayCommand]
        private void Search() => LoadLessen();

        // Vernieuw lessen via synchronizer
        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await _synchronizer.SynchronizeAll();  // Sync met API
            LoadLessen();  // Herlaad lessen
            await Task.Delay(500);
            IsRefreshing = false;
        }

        // Schrijf gebruiker in voor les
        [RelayCommand]
        private async Task Inschrijven(LocalLes les)
        {
            if (les == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Inschrijven",
                $"Weet je zeker dat je wilt inschrijven voor '{les.Naam}'?\n" +
                $"Datum: {les.StartTijd:dd/MM/yyyy HH:mm}\n" +
                $"Trainer: {les.Trainer}",  // Bevestigingsdialoog
                "Ja", "Nee");

            if (confirm)
            {
                try
                {
                    if (string.IsNullOrEmpty(General.UserId))  // Controleer of gebruiker ingelogd is
                    {
                        await Application.Current.MainPage.DisplayAlert("Fout", "Je moet ingelogd zijn om in te schrijven", "OK");
                        return;
                    }

                    var inschrijving = new LocalInschrijving
                    {
                        LesId = les.Id,
                        GebruikerId = General.UserId,
                        InschrijfDatum = DateTime.Now,
                        Status = "Actief"
                    };

                    _context.Inschrijvingen.Add(inschrijving);  // Voeg nieuwe inschrijving toe
                    await _context.SaveChangesAsync();  // Sla op in database

                    await Application.Current.MainPage.DisplayAlert("Succes", "Succesvol ingeschreven!", "OK");
                    LoadLessen();  // Herlaad lessen lijst
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", $"Inschrijven mislukt: {ex.Message}", "OK");
                }
            }
        }

        // Toon details van specifieke les
        [RelayCommand]
        private async Task ViewLessonDetails(LocalLes les)
        {
            if (les == null) return;

            var actieveInschrijvingen = les.AantalIngeschreven;  // Gebruik berekende property
            var beschikbaar = les.MaxDeelnemers - actieveInschrijvingen;  // Bereken beschikbare plaatsen

            await Application.Current.MainPage.DisplayAlert(
                les.Naam,
                $"Trainer: {les.Trainer}\n" +
                $"Datum: {les.StartTijd:dd/MM/yyyy HH:mm}\n" +
                $"Locatie: {les.Locatie}\n" +
                $"Beschikbaar: {beschikbaar}/{les.MaxDeelnemers}\n" +
                $"Duur: {(les.EindTijd - les.StartTijd).TotalMinutes} minuten\n\n" +
                $"{les.Beschrijving}",  // Toon alle lesdetails
                "OK");
        }
    }
}