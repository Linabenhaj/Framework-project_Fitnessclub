using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class AdminDashboardPage : ContentPage
    {
        private readonly AdminDashboardViewModel? _viewModel;

        public AdminDashboardPage(AdminDashboardViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
            Shell.SetBackgroundColor(this, Color.FromArgb("#1B5E20"));
            Shell.SetTitleColor(this, Colors.White);
            Shell.SetForegroundColor(this, Colors.White);
        }

        public AdminDashboardPage() { InitializeComponent(); }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = _viewModel?.LoadStatsAsync();
        }
    }
}
