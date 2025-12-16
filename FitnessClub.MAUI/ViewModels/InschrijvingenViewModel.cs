using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class InschrijvingenViewModel : BaseViewModel
    {
        private readonly LocalDbContext _context;  // LocalDbContext ipv FitnessClubDbContext

        [ObservableProperty]
        private ObservableCollection<LocalInschrijving> mijnInschrijvingen = new();  // LocalInschrijving ipv Inschrijving

        [ObservableProperty]
        private bool toonAlleenActief = true;

        public InschrijvingenViewModel(LocalDbContext context)
        {
            _context = context;
            Title = "Mijn Inschrijvingen";
            LoadInschrijvingen();
        }

        public async void LoadInschrijvingen()
        {
            if (IsBusy) return;
            IsBusy = true;
            MijnInschrijvingen.Clear();

            try
            {
                await General.LoadUserInfo();

                if (string.IsNullOrEmpty(General.UserId))
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "Log in om je inschrijvingen te zien", "OK");
                    return;
                }

                var query = _context.Inschrijvingen
                    .Include(i => i.Les)
                    .Where(i => i.GebruikerId == General.UserId);

                if (ToonAlleenActief)
                    query = query.Where(i => i.Status == "Actief" && i.Les.StartTijd > DateTime.Now);

                var inschrijvingen = await query.OrderByDescending(i => i.Les.StartTijd).ToListAsync();

                foreach (var inschrijving in inschrijvingen)
                    MijnInschrijvingen.Add(inschrijving);
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

        [RelayCommand]
        private async Task Uitschrijven(LocalInschrijving inschrijving)  // LocalInschrijving ipv Inschrijving
        {
            if (inschrijving == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Uitschrijven",
                $"Weet je zeker dat je wilt uitschrijven voor '{inschrijving.Les?.Naam}'?",
                "Ja", "Nee");

            if (confirm)
            {
                try
                {
                    if (inschrijving.Les != null && inschrijving.Les.StartTijd <= DateTime.Now.AddHours(24))
                    {
                        await Application.Current.MainPage.DisplayAlert("Fout", "Uitschrijven is alleen mogelijk tot 24 uur voor de les", "OK");
                        return;
                    }

                    inschrijving.Status = "Geannuleerd";
                    _context.Inschrijvingen.Update(inschrijving);
                    await _context.SaveChangesAsync();

                    MijnInschrijvingen.Remove(inschrijving);
                    await Application.Current.MainPage.DisplayAlert("Succes", "Succesvol uitgeschreven", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", $"Uitschrijven mislukt: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task ViewLesDetails(LocalInschrijving inschrijving)  // LocalInschrijving ipv Inschrijving
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
                $"Beschrijving: {les.Beschrijving}",
                "OK");
        }

        [RelayCommand]
        private void FilterChanged() => LoadInschrijvingen();

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            LoadInschrijvingen();
            await Task.Delay(500);
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task NieuweInschrijving() => await Shell.Current.GoToAsync("//LessenPage");
    }
}