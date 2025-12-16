using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly Synchronizer _synchronizer;
        private readonly LocalDbContext _context;  // LocalDbContext ipv FitnessClubDbContext

        [ObservableProperty]
        private string welcomeMessage = "Welkom bij FitnessClub!";

        [ObservableProperty]
        private int activeLessonsCount = 0;

        [ObservableProperty]
        private int myRegistrationsCount = 0;

        [ObservableProperty]
        private ObservableCollection<LocalLes> upcomingLessons = new();  // LocalLes ipv Les

        public HomeViewModel(Synchronizer synchronizer, LocalDbContext context)
        {
            _synchronizer = synchronizer;
            _context = context;
            Title = "Dashboard";
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                await General.LoadUserInfo();

                if (!string.IsNullOrEmpty(General.UserFirstName))
                    WelcomeMessage = $"Welkom, {General.UserFirstName}!";

                ActiveLessonsCount = await _context.Lessen
                    .Where(l => l.IsActief && l.StartTijd > DateTime.Now)
                    .CountAsync();

                if (!string.IsNullOrEmpty(General.UserId))
                {
                    MyRegistrationsCount = await _context.Inschrijvingen
                        .Where(i => i.GebruikerId == General.UserId &&
                                   i.Status == "Actief" &&
                                   i.Les.StartTijd > DateTime.Now)
                        .CountAsync();
                }

                // Load upcoming lessons
                UpcomingLessons.Clear();
                var lessons = await _context.Lessen
                    .Where(l => l.IsActief && l.StartTijd > DateTime.Now)
                    .OrderBy(l => l.StartTijd)
                    .Take(3)
                    .ToListAsync();

                foreach (var lesson in lessons)
                    UpcomingLessons.Add(lesson);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading dashboard: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToLessons() => await Shell.Current.GoToAsync("//LessenPage");

        [RelayCommand]
        private async Task NavigateToRegistrations() => await Shell.Current.GoToAsync("//InschrijvingenPage");

        [RelayCommand]
        private async Task ManualSync()
        {
            IsBusy = true;
            try
            {
                await _synchronizer.SynchronizeAll();
                LoadDashboardData();
                await Application.Current.MainPage.DisplayAlert("Succes", "Synchronisatie voltooid!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", $"Synchronisatie mislukt: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");

            if (confirm)
            {
                _synchronizer.Logout();
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}