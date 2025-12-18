using FitnessClub.MAUI.Services;

namespace FitnessClub.MAUI.Views
{
    public partial class AdminDashboardPage : ContentPage
    {
        public AdminDashboardPage()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Uitloggen",
                "Weet je zeker dat je wilt uitloggen?",
                "Ja", "Nee");

            if (confirm)
            {
                General.ClearUserInfo();
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
    }
}