using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class ProfielPage : ContentPage
    {
        private readonly ProfielViewModel? _viewModel;

        public ProfielPage(ProfielViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
            Shell.SetBackgroundColor(this, Color.FromArgb("#1B5E20"));
            Shell.SetTitleColor(this, Colors.White);
            Shell.SetForegroundColor(this, Colors.White);
        }

        public ProfielPage() { InitializeComponent(); }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Herlaad gegevens telkens de pagina verschijnt : vooral belangrijk voor net-geregistreerde gebruikers
            if (_viewModel != null)
                await _viewModel.RefreshCommand.ExecuteAsync(null);
        }

        private async void OnBackClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("..");
    }
}
