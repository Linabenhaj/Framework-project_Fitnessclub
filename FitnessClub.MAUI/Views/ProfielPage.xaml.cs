using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class ProfielPage : ContentPage
    {
        public ProfielPage(ProfielViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}