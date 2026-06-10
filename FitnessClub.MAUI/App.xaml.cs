using FitnessClub.MAUI.Views;

namespace FitnessClub.MAUI
{
    // Hoofdklasse van de applicatie
    public partial class App : Application
    {
        public App(AppShell shell)
        {
            InitializeComponent();
            MainPage = shell;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await ToonGdprAlsNoodigAsync();
        }

        private static async Task ToonGdprAlsNoodigAsync()
        {
            if (Preferences.Default.Get("gdpr_accepted", false))
                return;

            await Task.Delay(600);

            var page = Application.Current?.Windows[0]?.Page;
            if (page == null) return;

            bool accepted = await page.DisplayAlert(
                "Privacymelding",
                "FitnessClub slaat je e-mailadres en inloggegevens lokaal op zodat je " +
                "niet telkens opnieuw hoeft in te loggen.\n\n" +
                "Je gegevens worden NIET gedeeld met derden en zijn enkel zichtbaar op dit apparaat.\n\n" +
                "Ga je akkoord met het lokaal opslaan van je gegevens?",
                "Akkoord",
                "Weigeren");

            Preferences.Default.Set("gdpr_accepted", accepted);

            if (!accepted)
            {
                General.ClearUserInfo();
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
        }
    }
}
