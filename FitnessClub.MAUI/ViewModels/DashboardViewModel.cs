using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.Models.Models;
using FitnessClub.MAUI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string welcomeMessage = "Welkom bij FitnessClub!";

        [ObservableProperty]
        private int activeLessonsCount = 0;

        [ObservableProperty]
        private int myRegistrationsCount = 0;

        [ObservableProperty]
        private ObservableCollection<LocalLes> upcomingLessons = [];

        public DashboardViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Dashboard";
            _ = LoadDashboardDataAsync();
        }

        public async Task LoadDashboardDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (!string.IsNullOrEmpty(General.UserFirstName))
                    WelcomeMessage = $"Welkom, {General.UserFirstName}!";

                var lessenResult = await _apiService.GetAllLessenAsync();
                if (lessenResult.Success && lessenResult.Data != null)
                {
                    var toekomstig = lessenResult.Data
                        .Where(l => l.IsActief && l.StartTijd > DateTime.Now)
                        .OrderBy(l => l.StartTijd)
                        .ToList();

                    ActiveLessonsCount = toekomstig.Count;

                    UpcomingLessons.Clear();
                    foreach (var les in toekomstig.Take(3))
                        UpcomingLessons.Add(les);
                }

                var insResult = await _apiService.GetUserInschrijvingenAsync(General.UserId);
                if (insResult.Success && insResult.Data != null)
                {
                    MyRegistrationsCount = insResult.Data
                        .Count(i => i.Status == "Actief");
                }

                Debug.WriteLine("Dashboard data geladen via API");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fout bij laden dashboard: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToLessons()
        {
            await Shell.Current.GoToAsync("LessenPage");
        }

        [RelayCommand]
        private async Task NavigateToRegistrations()
        {
            await Shell.Current.GoToAsync("InschrijvingenPage");
        }

        [RelayCommand]
        private async Task NavigateToProfile()
        {
            await Shell.Current.GoToAsync("ProfielPage");
        }

        [RelayCommand]
        private async Task ManualSync()
        {
            await LoadDashboardDataAsync();
            await Application.Current!.Windows[0]!.Page!.DisplayAlert("Succes", "Gegevens vernieuwd!", "OK");
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Application.Current!.Windows[0]!.Page!.DisplayAlert(
                "Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");

            if (confirm)
            {
                General.ClearUserInfo();
                _apiService.SetToken(null);
                await Shell.Current.GoToAsync("//HomePage");
            }
        }

        [RelayCommand]
        private async Task RefreshDashboard()
        {
            await LoadDashboardDataAsync();
        }
    }
}
