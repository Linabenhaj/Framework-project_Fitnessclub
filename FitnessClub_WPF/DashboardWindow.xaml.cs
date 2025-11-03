using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class DashboardWindow : Window
    {
        private readonly Gebruiker _user;
        private readonly List<string> _roles;

        public DashboardWindow(Gebruiker user, List<string> roles)
        {
            InitializeComponent();
            _user = user;
            _roles = roles;

            LoadUserInfo();
            SetupRoleBasedTabs();
        }

        private void LoadUserInfo()
        {
            UserNameText.Text = $"{_user.Voornaam} {_user.Achternaam}";
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

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            var signInManager = App.ServiceProvider.GetRequiredService<SignInManager<Gebruiker>>();
            await signInManager.SignOutAsync();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}