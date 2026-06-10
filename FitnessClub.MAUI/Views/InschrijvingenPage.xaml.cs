using FitnessClub.Models.Models;
using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.Views
{
    public partial class InschrijvingenPage : ContentPage
    {
        private readonly InschrijvingenViewModel? _viewModel;

        public InschrijvingenPage(InschrijvingenViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
            Shell.SetBackgroundColor(this, Color.FromArgb("#1B5E20"));
            Shell.SetTitleColor(this, Colors.White);
            Shell.SetForegroundColor(this, Colors.White);

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

        public InschrijvingenPage() { InitializeComponent(); }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = _viewModel?.LoadInschrijvingenAsync();
        }

        private async void OnUitschrijvenClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is LocalInschrijving ins && _viewModel != null)
                await _viewModel.UitschrijvenCommand.ExecuteAsync(ins);
        }
    }
}
