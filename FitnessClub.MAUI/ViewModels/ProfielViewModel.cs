using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class ProfielViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string volledigeNaam = "Test Gebruiker";

        [ObservableProperty]
        private string email = "test@fitness.com";

        [ObservableProperty]
        private string telefoon = "012 34 56 78";

        [ObservableProperty]
        private DateTime geboortedatum = new(1990, 1, 1);  // ← FIXED

        [ObservableProperty]
        private string abonnementNaam = "Premium - €29.99/maand";

        [ObservableProperty]
        private int aantalInschrijvingen = 3;

        [ObservableProperty]
        private bool isEditing = false;

        public ProfielViewModel()
        {
            Title = "Mijn Profiel";
        }

        [RelayCommand]
        private void EditProfile() => IsEditing = true;

        [RelayCommand]
        private async Task SaveProfile()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(VolledigeNaam))
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", "Naam is verplicht", "OK");
                    return;
                }

                await Application.Current.MainPage.DisplayAlert("Succes",
                    $"Profiel opgeslagen!\nNaam: {VolledigeNaam}", "OK");

                IsEditing = false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout",
                    $"Kon profiel niet opslaan: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}