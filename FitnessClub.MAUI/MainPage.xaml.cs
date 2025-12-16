using Microsoft.Maui.Controls;

namespace FitnessClub.MAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            DisplayAlert("Alert", "Knop is geklikt!", "OK");
        }
    }
}
