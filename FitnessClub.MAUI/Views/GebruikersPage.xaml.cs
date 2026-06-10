using FitnessClub.MAUI.Services;
using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class GebruikersPage : ContentPage
    {
        private readonly GebruikersViewModel? _viewModel;

        public GebruikersPage(GebruikersViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
            Shell.SetBackgroundColor(this, Color.FromArgb("#1B5E20"));
            Shell.SetTitleColor(this, Colors.White);
            Shell.SetForegroundColor(this, Colors.White);

            // Terug-knop naar Beheer (admin)
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "← Beheer",
                Order = ToolbarItemOrder.Primary,
                Command = new Command(async () => await Shell.Current.GoToAsync("//AdminShell"))
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = _viewModel?.LoadGebruikersAsync();
        }

        private async void OnVerwijderClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is GebruikerInfo gebr && _viewModel != null)
                await _viewModel.VerwijderGebruikerCommand.ExecuteAsync(gebr);
        }
    }
}
