using FitnessClub.MAUI.Views.Admin;

namespace FitnessClub.MAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnAdminLoginClicked(object sender, EventArgs e)
        {
            // Direct naar admin dashboard
            await Navigation.PushAsync(new AdminDashboardPage());
        }

        private async void OnTrainerLoginClicked(object sender, EventArgs e)
        {
            // Naar home page
            await Navigation.PushAsync(new HomePage());
        }

        private async void OnUserLoginClicked(object sender, EventArgs e)
        {
            // Naar home page
            await Navigation.PushAsync(new HomePage());
        }
    }
}