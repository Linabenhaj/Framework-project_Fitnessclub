namespace FitnessClub.MAUI.Views.Admin
{
    public partial class AdminDashboardPage : ContentPage
    {
        public AdminDashboardPage()
        {
            InitializeComponent();
        }

        private async void OnManageLessonsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.LessenPage());
        }

        private async void OnGoToHomeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.HomePage());
        }

        private async void OnGoToLessonsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.LessenPage());
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnManageUsersClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Gebruikers", "Functie werkt!", "OK");
        }

        private async void OnManageSubscriptionsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Abonnementen", "Functie werkt!", "OK");
        }

        private async void OnViewReportsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Rapporten", "Functie werkt!", "OK");
        }
    }
}