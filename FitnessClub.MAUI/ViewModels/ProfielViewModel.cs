using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class ProfielViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string volledigeNaam = "John Doe";

        [ObservableProperty]
        private string email = "john.doe@fitness.com";

        [ObservableProperty]
        private string telefoon = "012 34 56 78";

        [ObservableProperty]
        private DateTime geboortedatum = new DateTime(1990, 1, 1);

        [ObservableProperty]
        private string abonnementNaam = "Premium - €29.99/maand";

        [ObservableProperty]
        private int aantalInschrijvingen = 3;

        [ObservableProperty]
        private bool isEditing = false;

        //  Constructor ZONDER LocalDbContext
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
                // Simpele validatie
                if (string.IsNullOrWhiteSpace(VolledigeNaam))
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", "Naam is verplicht", "OK");
                    return;
                }

                // Demo opslag
                await Application.Current.MainPage.DisplayAlert("Succes",
                    $"Profiel opgeslagen!\nNaam: {VolledigeNaam}\nEmail: {Email}", "OK");

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

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditing = false;
            // Reset naar demo waarden
            VolledigeNaam = "John Doe";
            Email = "john.doe@fitness.com";
            Telefoon = "012 34 56 78";
            AbonnementNaam = "Premium - €29.99/maand";
        }
    }
}