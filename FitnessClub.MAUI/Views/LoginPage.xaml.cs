using Microsoft.Maui.Controls;

namespace FitnessClub.MAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ErrorLabel.Text = "Vul email en wachtwoord in";
                ErrorLabel.IsVisible = true;
                return;
            }

            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;

            await Task.Delay(1000);

            if ((EmailEntry.Text == "admin@fitness.com" && PasswordEntry.Text == "Admin123!") ||
                (EmailEntry.Text == "lid@fitness.com" && PasswordEntry.Text == "Lid123!"))
            {
                await DisplayAlert("Succes", "Login geslaagd! (Demo)", "OK");
            }
            else
            {
                ErrorLabel.Text = "Ongeldige login gegevens. Gebruik demo accounts.";
                ErrorLabel.IsVisible = true;
            }

            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}