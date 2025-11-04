using System.Collections.Generic;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class DashboardWindow : Window
    {
        private readonly List<string> _roles;

        public DashboardWindow(object user, List<string> roles)
        {
            InitializeComponent();
            _roles = roles;

            LoadUserInfo();
            SetupRoleBasedTabs();
        }

        private void LoadUserInfo()
        {
            UserNameText.Text = "Demo Gebruiker";
            RoleInfoText.Text = $"Rol: {string.Join(", ", _roles)}";
        }

        private void SetupRoleBasedTabs()
        {
            // Menu aanpassen per rol
            if (_roles.Contains("Admin"))
            {
                LedenTab.Visibility = Visibility.Visible;
                AbonnementTab.Visibility = Visibility.Visible;
                InstellingenTab.Visibility = Visibility.Visible;
            }
            if (_roles.Contains("Lid"))
            {
                AbonnementTab.Visibility = Visibility.Visible;
                InstellingenTab.Visibility = Visibility.Visible;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}