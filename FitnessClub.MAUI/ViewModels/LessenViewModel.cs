using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class LessenViewModel : BaseViewModel
    {
        private readonly LocalDbContext _context;  // LocalDbContext ipv FitnessClubDbContext
        private readonly Synchronizer _synchronizer;

        [ObservableProperty]
        private ObservableCollection<LocalLes> lessen = new();  // LocalLes ipv Les

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private bool showOnlyAvailable = true;

        public LessenViewModel(LocalDbContext context, Synchronizer synchronizer)
        {
            _context = context;
            _synchronizer = synchronizer;
            Title = "Lessen";
            LoadLessen();
        }

        public async void LoadLessen()
        {
            if (IsBusy) return;
            IsBusy = true;
            Lessen.Clear();

            try
            {
                var query = _context.Lessen
                    .Include(l => l.Inschrijvingen)
                    .Where(l => l.IsActief && l.StartTijd > DateTime.Now);

                if (ShowOnlyAvailable)
                    query = query.Where(l => l.Inschrijvingen.Count(i => i.Status == "Actief") < l.MaxDeelnemers);

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(l =>
                        l.Naam.Contains(SearchText) ||
                        l.Trainer.Contains(SearchText) ||
                        l.Locatie.Contains(SearchText));
                }

                var lessonList = await query.OrderBy(l => l.StartTijd).ToListAsync();
                foreach (var les in lessonList)
                    Lessen.Add(les);
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

        [RelayCommand]
        private void Search() => LoadLessen();

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await _synchronizer.SynchronizeAll();
            LoadLessen();
            await Task.Delay(500);
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task Inschrijven(LocalLes les)  // LocalLes ipv Les
        {
            if (les == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Inschrijven",
                $"Weet je zeker dat je wilt inschrijven voor '{les.Naam}'?\n" +
                $"Datum: {les.StartTijd:dd/MM/yyyy HH:mm}\n" +
                $"Trainer: {les.Trainer}",
                "Ja", "Nee");

            if (confirm)
            {
                try
                {
                    var inschrijving = new LocalInschrijving  // LocalInschrijving ipv Inschrijving
                    {
                        LesId = les.Id,
                        GebruikerId = General.UserId,
                        InschrijfDatum = DateTime.Now,
                        Status = "Actief"
                    };

                    _context.Inschrijvingen.Add(inschrijving);
                    await _context.SaveChangesAsync();

                    await Application.Current.MainPage.DisplayAlert("Succes", "Succesvol ingeschreven!", "OK");
                    LoadLessen();
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", $"Inschrijven mislukt: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task ViewLessonDetails(LocalLes les)  // LocalLes ipv Les
        {
            if (les == null) return;

            var actieveInschrijvingen = les.Inschrijvingen?.Count(i => i.Status == "Actief") ?? 0;
            var beschikbaar = les.MaxDeelnemers - actieveInschrijvingen;

            await Application.Current.MainPage.DisplayAlert(
                les.Naam,
                $"Trainer: {les.Trainer}\n" +
                $"Datum: {les.StartTijd:dd/MM/yyyy HH:mm}\n" +
                $"Locatie: {les.Locatie}\n" +
                $"Beschikbaar: {beschikbaar}/{les.MaxDeelnemers}\n" +
                $"Duur: {(les.EindTijd - les.StartTijd).TotalMinutes} minuten\n\n" +
                $"{les.Beschrijving}",
                "OK");
        }
    }
}