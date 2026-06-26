using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.Models.Models;
using FitnessClub.Models.Data;
using FitnessClub.MAUI.Services;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.MAUI.ViewModels
{
    // ViewModel voor de inschrijvingenpagina
    public partial class InschrijvingenViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly LocalDbContext _db;

        [ObservableProperty] private ObservableCollection<LocalInschrijving> mijnInschrijvingen = new();
        [ObservableProperty] private bool toonAlleenActief = true;
        [ObservableProperty] private bool isRefreshing;

        public string HeaderTitel => General.IsAdmin ? "Ingeschreven gebruikers" : "Jouw actieve inschrijvingen";

        private List<LocalInschrijving> _allInschrijvingen = new();

        public InschrijvingenViewModel(ApiService apiService, LocalDbContext db)
        {
            _apiService = apiService;
            _db = db;
            Title = General.IsAdmin ? "Alle inschrijvingen" : "Mijn inschrijvingen";
            _ = LoadInschrijvingenAsync();
        }

        public async Task LoadInschrijvingenAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            MijnInschrijvingen.Clear();

            try
            {
                if (General.IsAdmin)
                {
                    var result = await _apiService.GetAllBookingsAsync(); 
                    if (result.Success && result.Data != null)
                    {
                        _allInschrijvingen = result.Data;
                        ApplyFilter();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Fout",
                            result.Message ?? "API niet bereikbaar", "OK");
                    }
                }
                else // Gebruiker is geen admin, laad alleen zijn eigen inschrijvingen
                {
                    var cached = await _db.Inschrijvingen
                        .Where(i => i.GebruikerId == General.UserId)
                        .ToListAsync();
                    if (cached.Count > 0)
                    {
                        _allInschrijvingen = cached;
                        ApplyFilter();
                    }

                    // Probeer de inschrijvingen van de API te laden, maar als dat mislukt, gebruik de cache
                    var result = await _apiService.GetUserInschrijvingenAsync(General.UserId); //
                    if (result.Success && result.Data != null)
                    {
                        var lessenResult = await _apiService.GetAllLessenAsync();
                        if (lessenResult.Success && lessenResult.Data != null)
                        {
                            foreach (var ins in result.Data)
                                ins.Les = lessenResult.Data.FirstOrDefault(l => l.Id == ins.LesId);
                        }

                        _allInschrijvingen = result.Data;
                        ApplyFilter();

                        await SyncToSqliteAsync(result.Data);
                    }
                    else if (cached.Count == 0) // Als er geen cache is en de API niet bereikbaar is, toon een foutmelding
                    {
                        await Shell.Current.DisplayAlert("Fout",
                            result.Message ?? "API niet bereikbaar", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Fout", $"Fout bij laden: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilter()// past de filter toe op de lijst van inschrijvingen
        {
            MijnInschrijvingen.Clear();
            var filtered = _allInschrijvingen.AsEnumerable();

            if (ToonAlleenActief)
                filtered = filtered.Where(i => i.Status == "Actief");

            foreach (var ins in filtered.OrderByDescending(i => i.InschrijfDatum))
                MijnInschrijvingen.Add(ins);
        }

        private async Task SyncToSqliteAsync(List<LocalInschrijving> inschrijvingen) // synchroniseert de inschrijvingen met de lokale SQLite database
        {
            try
            {
                foreach (var ins in inschrijvingen)
                {
                    var existing = await _db.Inschrijvingen.FindAsync(ins.Id);
                    if (existing == null)  // als de inschrijving nog niet bestaat in de lokale database, voeg deze toe
                        _db.Inschrijvingen.Add(ins);
                    else
                        existing.Status = ins.Status;
                }
                await _db.SaveChangesAsync();
            }
            catch { }
        }

        [RelayCommand]
        private async Task Uitschrijven(LocalInschrijving inschrijving)
        {
            if (inschrijving == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Uitschrijven",
                $"Uitschrijven voor '{inschrijving.Les?.Naam ?? "deze les"}'?",
                "Ja", "Nee");

            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _apiService.DeleteInschrijvingAsync(inschrijving.Id);
                if (result.Success)
                {
                    MijnInschrijvingen.Remove(inschrijving);
                    _allInschrijvingen.RemoveAll(i => i.Id == inschrijving.Id);

                    await Shell.Current.DisplayAlert("Succes", "Je bent uitgeschreven", "OK");

                    _ = LoadInschrijvingenAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Fout",
                        result.Message ?? "Uitschrijven mislukt", "OK");
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void FilterChanged() => ApplyFilter();

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadInschrijvingenAsync();
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task NieuweInschrijving() => await Shell.Current.GoToAsync("//LessenShell");
    }
}
