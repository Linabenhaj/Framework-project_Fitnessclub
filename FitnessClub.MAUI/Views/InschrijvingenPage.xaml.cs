namespace FitnessClub.MAUI.Views
{
    public partial class InschrijvingenPage : ContentPage
    {
        public InschrijvingenPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}