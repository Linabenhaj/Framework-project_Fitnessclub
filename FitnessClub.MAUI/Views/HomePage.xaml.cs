using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}