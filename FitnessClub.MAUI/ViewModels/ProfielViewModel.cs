using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class ProfielViewModel : BaseViewModel  // ViewModel voor profiel pagina
    {
        [ObservableProperty]
        private string volledigeNaam = "Test Gebruiker";  // Gebruikersnaam

        [ObservableProperty]
        private string email = "test@fitness.com";  // Email adres

        [ObservableProperty]
        private string telefoon = "012 34 56 78";  // Telefoonnummer

        [ObservableProperty]
        private DateTime geboortedatum = new(1990, 1, 1);  // Geboortedatum

        [ObservableProperty]
        private string abonnementNaam = "Premium - €29.99/maand";  // Abonnementsinfo

        [ObservableProperty]
        private int aantalInschrijvingen = 3;  // Aantal actieve inschrijvingen

        [ObservableProperty]
        private bool isEditing = false;  // Bewerkmodus status

        public ProfielViewModel()
        {
            Title = "Mijn Profiel";
        }

        // Activeer bewerkmodus
        [RelayCommand]
        private void EditProfile() => IsEditing = true;

        // Sla profiel wijzigingen op
        [RelayCommand]
        private async Task SaveProfile()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(VolledigeNaam))  // Valideer verplichte velden
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", "Naam is verplicht", "OK");
                    return;
                }

                await Application.Current.MainPage.DisplayAlert("Succes",
                    $"Profiel opgeslagen!\nNaam: {VolledigeNaam}", "OK");

                IsEditing = false;  // Schakel bewerkmodus uit
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