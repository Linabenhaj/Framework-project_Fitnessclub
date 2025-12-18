using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage(DashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refresh data wanneer pagina verschijnt
            if (BindingContext is DashboardViewModel viewModel)
            {
                
            }
        }
    }
}