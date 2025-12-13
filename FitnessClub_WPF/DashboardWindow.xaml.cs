using FitnessClub.Models.Models;
using System.Collections.Generic;
using System.Windows;

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

            bool isAdmin = _roles.Contains("Admin");
            bool isLid = _roles.Contains("Lid");

            // Toon welke rollen gedetecteerd worden
            System.Diagnostics.Debug.WriteLine($"Dashboard - Rollen: {string.Join(", ", _roles)}, IsAdmin: {isAdmin}, IsLid: {isLid}");

            if (isAdmin)
            {
                SetupAdminTabs();
            }
            else if (isLid)
            {
                SetupLidTabs();
            }
            else
            {
                // Fallback - toon lid tabs
                SetupLidTabs();
            }

            // Profiel altijd zichtbaar (zonder speciale ProfielView)
            ProfielTab.Visibility = Visibility.Visible;
            // ProfielTab.Content wordt al geladen via XAML met de ProfileInfoText
        }

        private void SetupAdminTabs()
        {
            // TOON Admin beheer tabs
            LedenTab.Visibility = Visibility.Visible;
            AbonnementTab.Visibility = Visibility.Visible;
            InschrijvingenTab.Visibility = Visibility.Visible;
            LessenTab.Visibility = Visibility.Visible;

            // LAAD Admin views met volledige functionaliteit
            LedenTab.Content = new LedenOverzicht();         
            AbonnementTab.Content = new AbonnementenOverzicht(); 
            InschrijvingenTab.Content = new InschrijvingenOverzicht(); 
            LessenTab.Content = new LessenOverzicht();         

            // VERBERG Lid tabs
            MijnAbonnementTab.Visibility = Visibility.Collapsed;
            MijnInschrijvingenTab.Visibility = Visibility.Collapsed;
            LessenLidTab.Visibility = Visibility.Collapsed;

            // Selecteer eerste admin tab
            MainTabControl.SelectedItem = LedenTab;
        }

        private void SetupLidTabs()
        {
            // TOON Lid functionaliteit tabs
            MijnAbonnementTab.Visibility = Visibility.Visible;
            MijnInschrijvingenTab.Visibility = Visibility.Visible;
            LessenLidTab.Visibility = Visibility.Visible;

            // LAAD Lid views
            MijnAbonnementTab.Content = new MijnAbonnement();

            var mijnInschrijvingen = new MijnInschrijvingen();
            mijnInschrijvingen.SetHuidigeGebruiker(_user.Id);
            MijnInschrijvingenTab.Content = mijnInschrijvingen;

            LessenLidTab.Content = new LessenOverzichtLid();

            // VERBERG Admin tabs
            LedenTab.Visibility = Visibility.Collapsed;
            AbonnementTab.Visibility = Visibility.Collapsed;
            InschrijvingenTab.Visibility = Visibility.Collapsed;
            LessenTab.Visibility = Visibility.Collapsed;

            // Selecteer eerste lid tab
            MainTabControl.SelectedItem = MijnAbonnementTab;
        }

        public string GetCurrentUserId()
        {
            return _user?.Id;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}