using FitnessClub.Models.Models;
using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class LessenPage : ContentPage
    {
        private readonly LessenViewModel? _viewModel;

        public LessenPage(LessenViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
            Shell.SetBackgroundColor(this, Color.FromArgb("#1B5E20"));
            Shell.SetTitleColor(this, Colors.White);
            Shell.SetForegroundColor(this, Colors.White);

            // Terug-knop naar Beheer (alleen voor admin)
            if (General.IsAdmin)
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = "← Beheer",
                    Order = ToolbarItemOrder.Primary,
                    Command = new Command(async () => await Shell.Current.GoToAsync("//AdminShell"))
                });
            }
        }

        public LessenPage() { InitializeComponent(); }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = _viewModel?.LoadLessenAsync();
        }

        private async void OnInschrijvenClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is LocalLes les && _viewModel != null)
                await _viewModel.InschrijvenCommand.ExecuteAsync(les);
        }

        private async void OnVerwijderenClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is LocalLes les && _viewModel != null)
                await _viewModel.VerwijderLesCommand.ExecuteAsync(les);
        }

        private async void OnBewerkClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is LocalLes les && _viewModel != null)
                await _viewModel.BewerkLesCommand.ExecuteAsync(les);
        }

        private async void OnClaimClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is LocalLes les && _viewModel != null)
                await _viewModel.ClaimLesCommand.ExecuteAsync(les);
        }
    }
}
