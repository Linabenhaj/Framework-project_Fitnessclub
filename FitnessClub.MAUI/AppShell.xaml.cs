using FitnessClub.MAUI.Services;
using FitnessClub.MAUI.ViewModels;
using FitnessClub.MAUI.Views;

namespace FitnessClub.MAUI
{
    // Hoofdnavigatie van de app
    public partial class AppShell : Shell
    {
        public static AppShell? Instance { get; private set; }

        private readonly SecurityViewModel _security;
        private readonly ApiService _apiService;

        public AppShell(SecurityViewModel security, ApiService apiService)
        {
            InitializeComponent();
            Instance = this;
            _security = security;
            _apiService = apiService;

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));

            FlyoutBehavior = FlyoutBehavior.Disabled;

            Loaded += async (_, _) =>
            {
                if (!General.IsLoggedIn)
                {
                    await GoToAsync(nameof(LoginPage));
                }
                else
                {
                    bool tokenGeldig = await _apiService.ValidateTokenAsync();
                    if (tokenGeldig)
                        await RestoreSession();
                    else
                    {
                        General.ClearUserInfo();
                        _apiService.SetToken(null);
                        await GoToAsync(nameof(LoginPage));
                    }
                }
            };
        }

        public async Task OnLoginSucceeded(string userId, string email, string firstName, string lastName,
                                           string role, string token)
        {
            General.SaveUserInfo(userId, email, firstName, lastName, role, token);
            _apiService.SetToken(token);

            bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            bool isTrainer = role.Equals("Trainer", StringComparison.OrdinalIgnoreCase);
            _security.SetUser(email, isAdmin, $"{firstName} {lastName}".Trim());

            FlyoutBehavior = FlyoutBehavior.Flyout;

            AdminDashboardItem.IsVisible = isAdmin;
            GebruikersItem.IsVisible = isAdmin;
            ProfielItem.IsVisible = !isAdmin;
            InschrijvingenItem.IsVisible = !isTrainer;

            UserNameLabel.Text = string.IsNullOrEmpty(firstName)
                ? email
                : $"{firstName} {lastName}".Trim();

            if (isAdmin)
                await GoToAsync("//AdminShell");
            else
                await GoToAsync("//LessenShell");
        }

        private async Task RestoreSession()
        {
            bool isAdmin = General.IsAdmin;
            bool isTrainer = General.IsTrainer;
            _security.SetUser(General.UserEmail, isAdmin,
                $"{General.UserFirstName} {General.UserLastName}".Trim());
            _apiService.SetToken(General.Token);

            FlyoutBehavior = FlyoutBehavior.Flyout;
            AdminDashboardItem.IsVisible = isAdmin;
            GebruikersItem.IsVisible = isAdmin;
            ProfielItem.IsVisible = !isAdmin;
            InschrijvingenItem.IsVisible = !isTrainer;
            // Admin heeft geen persoonlijk profiel (geen abonnement, geen inschrijvingen),
            // dus we verbergen "Mijn profiel" voor admin.
            ProfielItem.IsVisible = !isAdmin;
            UserNameLabel.Text = string.IsNullOrEmpty(General.UserFirstName)
                ? General.UserEmail
                : $"{General.UserFirstName} {General.UserLastName}".Trim();

            if (isAdmin)
                await GoToAsync("//AdminShell");
            else
                await GoToAsync("//LessenShell");
        }

        public void SetFlyoutVisible(bool visible)
        {
            FlyoutBehavior = visible ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlert(
                "Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");

            if (!confirm) return;

            General.ClearUserInfo();
            _apiService.SetToken(null);
            _security.Reset();

            AdminDashboardItem.IsVisible = false;
            GebruikersItem.IsVisible = false;
            ProfielItem.IsVisible = true;
            InschrijvingenItem.IsVisible = true;
            UserNameLabel.Text = "Welkom";
            FlyoutBehavior = FlyoutBehavior.Disabled;

            await GoToAsync(nameof(LoginPage));
        }
    }
}
