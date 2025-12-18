using FitnessClub.MAUI.Services;

namespace FitnessClub.MAUI.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Valideer velden
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
                string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Fout", "Vul alle velden in", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Fout", "Wachtwoorden komen niet overeen", "OK");
                return;
            }

            if (PasswordEntry.Text.Length < 6)
            {
                await DisplayAlert("Fout", "Wachtwoord moet minimaal 6 tekens zijn", "OK");
                return;
            }

            // Toon succes bericht
            await DisplayAlert("Succes",
                $"Account aangemaakt voor:\n{FirstNameEntry.Text} {LastNameEntry.Text}\n{EmailEntry.Text}",
                "OK");

            // Ga terug naar login
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}