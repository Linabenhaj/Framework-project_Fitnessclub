using FitnessClub.MAUI.Views;

namespace FitnessClub.MAUI
{
    public static class AppShellRoutes
    {
        public static void RegisterRoutes()
        {
            // Authentication
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));

            // Admin Routes
            Routing.RegisterRoute("AdminDashboardPage", typeof(Views.Admin.AdminDashboardPage));

            // Main App Pages
            Routing.RegisterRoute("HomePage", typeof(HomePage));
            Routing.RegisterRoute("LessenPage", typeof(LessenPage));
            Routing.RegisterRoute("InschrijvingenPage", typeof(InschrijvingenPage));
            Routing.RegisterRoute("ProfielPage", typeof(ProfielPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
            Routing.RegisterRoute("AboutPage", typeof(AboutPage));
        }
    }
}