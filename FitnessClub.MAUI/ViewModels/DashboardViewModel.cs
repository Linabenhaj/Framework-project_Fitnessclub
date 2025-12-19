using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Models;
using FitnessClub.MAUI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string welcomeMessage = "Welkom bij FitnessClub!";

        [ObservableProperty]
        private int activeLessonsCount = 0;

        [ObservableProperty]
        private int myRegistrationsCount = 0;

        [ObservableProperty]
        private ObservableCollection<LocalLes> upcomingLessons = [];

        public DashboardViewModel() // 🔴 LEEGE CONSTRUCTOR - GEEN dependencies!
        {
            Title = "Dashboard";
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (!string.IsNullOrEmpty(General.UserFirstName))
                    WelcomeMessage = $"Welkom, {General.UserFirstName}!";

                // Demo data 
                ActiveLessonsCount = 5;
                MyRegistrationsCount = 3;

                // Demo lessen
                UpcomingLessons.Clear();
                UpcomingLessons.Add(new LocalLes
                {
                    Naam = "Yoga Basics",
                    StartTijd = DateTime.Now.AddDays(1),
                    Trainer = "Anna",
                    Locatie = "Zaal 1"
                });
                UpcomingLessons.Add(new LocalLes
                {
                    Naam = "HIIT Training",
                    StartTijd = DateTime.Now.AddDays(2),
                    Trainer = "Mike",
                    Locatie = "Zaal 2"
                });

                Debug.WriteLine("✅ Dashboard data loaded (demo)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading dashboard: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToLessons()
        {
            await Shell.Current.GoToAsync("//LessenPage");
        }

        [RelayCommand]
        private async Task NavigateToRegistrations()
        {
            await Shell.Current.GoToAsync("//InschrijvingenPage");
        }

        [RelayCommand]
        private async Task NavigateToProfile()
        {
            await Shell.Current.GoToAsync("//ProfielPage");
        }

        [RelayCommand]
        private async Task ManualSync()
        {
            IsBusy = true;
            try
            {
                //  Simpele sync zonder database
                Debug.WriteLine("🔄 Manual sync started (demo)");
                await Task.Delay(1000); // Simulatie

                // Herlaad data
                LoadDashboardData();

                await Application.Current.MainPage.DisplayAlert("Succes",
                    "Synchronisatie voltooid! (demo)", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout",
                    $"Synchronisatie mislukt: {ex.Message}", "OK");
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
                General.ClearUserInfo();
                await Shell.Current.GoToAsync("//HomePage");
            }
        }

        [RelayCommand]
        private async Task RefreshDashboard()
        {
            LoadDashboardData();
        }
    }
}