namespace FitnessClub.MAUI.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            Title = "Home";
        }

        private async void OnBookLessonsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LessenPage());
        }

        private async void OnViewRegistrationsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InschrijvingenPage());
        }

        private async void OnViewProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfielPage());
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}