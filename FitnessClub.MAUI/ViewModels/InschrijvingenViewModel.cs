using FitnessClub.MAUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class InschrijvingenViewModel : BaseViewModel  // ViewModel voor inschrijvingen pagina
    {
        private readonly LocalDbContext _context;

        [ObservableProperty]
        private ObservableCollection<LocalInschrijving> mijnInschrijvingen = new();  // Gebruikersinschrijvingen collectie

        [ObservableProperty]
        private bool toonAlleenActief = true;  // Filter voor actieve inschrijvingen

        public InschrijvingenViewModel(LocalDbContext context)
        {
            _context = context;
            Title = "Mijn Inschrijvingen";
            LoadInschrijvingen();  // Laad inschrijvingen bij opstart
        }

        // Laad inschrijvingen van huidige gebruiker
        public async void LoadInschrijvingen()
        {
            if (IsBusy) return;
            IsBusy = true;
            MijnInschrijvingen.Clear();  // Maak lijst leeg

            try
            {
                if (string.IsNullOrEmpty(General.UserId))  // Controleer of gebruiker ingelogd is
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "Log in om je inschrijvingen te zien", "OK");
                    return;
                }

                var query = _context.Inschrijvingen
                    .Include(i => i.Les)  // Include les details
                    .Where(i => i.GebruikerId == General.UserId && i.Les != null);  // Filter op gebruiker

                if (ToonAlleenActief)
                    query = query.Where(i => i.Status == "Actief" && i.Les!.StartTijd > DateTime.Now);  // Filter actieve toekomstige lessen

                var inschrijvingen = await query.OrderByDescending(i => i.Les!.StartTijd).ToListAsync();  // Sorteer op datum

                foreach (var inschrijving in inschrijvingen)
                    MijnInschrijvingen.Add(inschrijving);  // Voeg toe aan observable collectie
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", $"Kon inschrijvingen niet laden: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Schrijf gebruiker uit van les
        [RelayCommand]
        private async Task Uitschrijven(LocalInschrijving inschrijving)
        {
            if (inschrijving == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Uitschrijven",
                $"Weet je zeker dat je wilt uitschrijven voor '{inschrijving.Les?.Naam}'?",  // Bevestigingsdialoog
                "Ja", "Nee");

            if (confirm)
            {
                try
                {
                    // Controleer uitschrijftermijn (24 uur voor les)
                    if (inschrijving.Les != null && inschrijving.Les.StartTijd <= DateTime.Now.AddHours(24))
                    {
                        await Application.Current.MainPage.DisplayAlert("Fout", "Uitschrijven is alleen mogelijk tot 24 uur voor de les", "OK");
                        return;
                    }

                    inschrijving.Status = "Geannuleerd";  // Update status
                    _context.Inschrijvingen.Update(inschrijving);
                    await _context.SaveChangesAsync();  // Sla wijzigingen op

                    MijnInschrijvingen.Remove(inschrijving);  // Verwijder uit lijst
                    await Application.Current.MainPage.DisplayAlert("Succes", "Succesvol uitgeschreven", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", $"Uitschrijven mislukt: {ex.Message}", "OK");
                }
            }
        }

        // Toon details van specifieke inschrijving
        [RelayCommand]
        private async Task ViewLesDetails(LocalInschrijving inschrijving)
        {
            if (inschrijving?.Les == null) return;

            var les = inschrijving.Les;
            await Application.Current.MainPage.DisplayAlert(
                les.Naam,
                $"Status: {inschrijving.Status}\n" +
                $"Inschrijfdatum: {inschrijving.InschrijfDatum:dd/MM/yyyy HH:mm}\n" +
                $"Les datum: {les.StartTijd:dd/MM/yyyy HH:mm}\n" +
                $"Trainer: {les.Trainer}\n" +
                $"Locatie: {les.Locatie}\n" +
                $"Beschrijving: {les.Beschrijving}",  // Toon alle details
                "OK");
        }

        // Herlaad inschrijvingen bij filter wijziging
        [RelayCommand]
        private void FilterChanged() => LoadInschrijvingen();

        // Vernieuw inschrijvingen lijst
        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            LoadInschrijvingen();
            await Task.Delay(500);
            IsRefreshing = false;
        }

        // Navigeer naar lessen pagina voor nieuwe inschrijving
        [RelayCommand]
        private async Task NieuweInschrijving() => await Shell.Current.GoToAsync("//LessenPage");
    }
}