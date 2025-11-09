using System.Collections.Generic;
using System.Windows;
using FitnessClub.Models;

namespace FitnessClub.WPF
{
    public partial class DashboardWindow : Window
    {
        private readonly Gebruiker _user;
        private readonly List<string> _roles;

        // alleen roles en user
        public DashboardWindow(List<string> roles, Gebruiker user)
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
            EmailText.Text = $"E-mail: {_user.Email}";
            FooterRoleText.Text = $"Rol: {string.Join(", ", _roles)}";

            // Toon uitgebreide profiel info
            ProfileInfoText.Text = $"Naam: {_user.Voornaam} {_user.Achternaam}\n" +
                                  $"E-mail: {_user.Email}\n" +
                                  $"Telefoon: {_user.Telefoon}\n" +
                                  $"Geboortedatum: {_user.Geboortedatum:dd/MM/yyyy}\n" +
                                  $"Rol: {string.Join(", ", _roles)}\n\n" +
                                  $"Welkom bij Fitness Club!";
        }

        private void SetupRoleBasedTabs()
        {
            // Reset alle tabs
            LedenTab.Visibility = Visibility.Collapsed;
            AbonnementTab.Visibility = Visibility.Collapsed;
            InschrijvingenTab.Visibility = Visibility.Collapsed;
            MijnAbonnementTab.Visibility = Visibility.Collapsed;
            MijnInschrijvingenTab.Visibility = Visibility.Collapsed;
            LessenTab.Visibility = Visibility.Collapsed;

            if (_roles.Contains("Admin"))
            {
                // Admin: Beheer tabs
                LedenTab.Visibility = Visibility.Visible;
                AbonnementTab.Visibility = Visibility.Visible;
                InschrijvingenTab.Visibility = Visibility.Visible;
                LessenTab.Visibility = Visibility.Visible; // ✅ Lessen Beheer voor Admin
            }
            else if (_roles.Contains("Lid"))
            {
                // Lid: Persoonlijke tabs
                MijnAbonnementTab.Visibility = Visibility.Visible;
                MijnInschrijvingenTab.Visibility = Visibility.Visible;
                // Lid krijgt een andere view voor lessen (beschikbare lessen)
            }

            // Profiel altijd zichtbaar
            ProfielTab.Visibility = Visibility.Visible;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}