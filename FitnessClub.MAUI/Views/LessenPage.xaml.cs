namespace FitnessClub.MAUI.Views
{
    public partial class LessenPage : ContentPage
    {
        public LessenPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}