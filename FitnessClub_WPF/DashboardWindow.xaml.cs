using System.Collections.Generic;
using System.Windows;
using FitnessClub.Models;
using FitnessClub.WPF.Views;

namespace FitnessClub.WPF
{
    public partial class DashboardWindow : Window
    {
        private readonly Gebruiker _user;
        private readonly List<string> _roles;

        public DashboardWindow(List<string> roles, Gebruiker user)
        {
            InitializeComponent();
            _user = user;
            _roles = roles;
            LoadUserInfo();
            SetupRoleBasedTabs();
        }

        public string GetCurrentUserId()
        {
            return _user?.Id;
        }

        private void LoadUserInfo()
        {
            UserNameText.Text = $"{_user.Voornaam} {_user.Achternaam}";
            EmailText.Text = $"E-mail: {_user.Email}";
            FooterRoleText.Text = $"Rol: {string.Join(", ", _roles)}";

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
            LessenLidTab.Visibility = Visibility.Collapsed;

            if (_roles.Contains("Admin"))
            {
                // Admin: Beheer tabs
                LedenTab.Visibility = Visibility.Visible;
                AbonnementTab.Visibility = Visibility.Visible;
                InschrijvingenTab.Visibility = Visibility.Visible;
                LessenTab.Visibility = Visibility.Visible;
            }
            else if (_roles.Contains("Lid"))
            {
                // Lid: Persoonlijke tabs + beschikbare lessen
                MijnAbonnementTab.Visibility = Visibility.Visible;
                MijnInschrijvingenTab.Visibility = Visibility.Visible;
                LessenLidTab.Visibility = Visibility.Visible;

                // DYNAMISCH CONTENT TOEVOEGEN
                SetupLidTabs();
            }

            // Profiel altijd zichtbaar
            ProfielTab.Visibility = Visibility.Visible;
        }

        private void SetupLidTabs()
        {
            // Mijn Abonnement tab
            var mijnAbonnementView = new MijnAbonnement();
            MijnAbonnementTab.Content = mijnAbonnementView;

            // Mijn Inschrijvingen tab
            var mijnInschrijvingenView = new MijnInschrijvingen();
            mijnInschrijvingenView.SetHuidigeGebruiker(_user.Id);
            MijnInschrijvingenTab.Content = mijnInschrijvingenView;

            
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}