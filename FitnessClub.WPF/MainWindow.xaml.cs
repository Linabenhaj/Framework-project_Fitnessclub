using FitnessClub.WPF.Views;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF
{
    public partial class MainWindow : Window
    {
        private string _currentUserRole;
        private string _currentUsername;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetUserRole(string role, string username)
        {
            _currentUserRole = role;
            _currentUsername = username;

            UpdateMenuForRole();
            ShowWelcomeMessage();
        }

        private void UpdateMenuForRole()
        {
            // RESET alles eerst
            mniLeden.Visibility = Visibility.Visible;
            mniAbonnementen.Visibility = Visibility.Visible;
            mniInschrijvingen.Visibility = Visibility.Visible;
            mniBetalingen.Visibility = Visibility.Visible;
            mniAdmin.Visibility = Visibility.Visible;

            // Rol-based toegang
            switch (_currentUserRole)
            {
                case "Lid":
                    // Leden zien ALLEEN leden overzicht (geen toevoegen)
                    mniLidToevoegen.Visibility = Visibility.Collapsed;
                    mniAbonnementen.Visibility = Visibility.Collapsed;
                    mniInschrijvingen.Visibility = Visibility.Collapsed;
                    mniBetalingen.Visibility = Visibility.Collapsed;
                    mniAdmin.Visibility = Visibility.Collapsed;
                    break;

                case "Medewerker":
                    // Medewerkers zien alles BEHALVE admin tools
                    mniAdmin.Visibility = Visibility.Collapsed;
                    break;

                case "Admin":
                    // Admin ziet ALLES
                    break;
            }

            this.Title = $"Fitness Club - {_currentUsername} ({_currentUserRole})";
        }

        private void ShowWelcomeMessage()
        {
            string welcomeMessage = _currentUserRole switch
            {
                "Lid" => $"Welkom {_currentUsername}! Bekijk hier je lidmaatschap.",
                "Medewerker" => $"Welkom {_currentUsername}! Beheer de fitness club.",
                "Admin" => $"Welkom {_currentUsername}! Volledig beheer toegang.",
                _ => "Welkom!"
            };

            // Toon welkomstscherm
            MainFrame.Content = new WelcomePage(welcomeMessage);
        }

        // Welkomstpagina class
        public class WelcomePage : Page
        {
            public WelcomePage(string message)
            {
                var stackPanel = new StackPanel
                {
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                };

                stackPanel.Children.Add(new TextBlock
                {
                    Text = message,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                });

                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Kies een optie uit het menu om te beginnen.",
                    FontSize = 14,
                    TextAlignment = TextAlignment.Center,
                    Foreground = System.Windows.Media.Brushes.Gray
                });

                this.Content = stackPanel;
            }
        }

        // Menu click handlers
        private void mniLedenOverzicht_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new LedenOverzicht();
        }

        private void mniLidToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserRole == "Lid")
            {
                MessageBox.Show("Alleen medewerkers en administrators kunnen leden toevoegen.");
                return;
            }
            var window = new LidToevoegen();
            window.Owner = this;
            window.ShowDialog();
            MainFrame.Content = new LedenOverzicht();
        }

        private void mniAbonnementenOverzicht_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AbonnementenOverzicht();
        }

        private void mniAbonnementToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new AbonnementToevoegen();
            window.Owner = this;
            window.ShowDialog();
            MainFrame.Content = new AbonnementenOverzicht();
        }

        private void mniInschrijvingenOverzicht_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new InschrijvingenOverzicht();
        }

        private void mniInschrijvingToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new InschrijvingToevoegen();
            window.Owner = this;
            window.ShowDialog();
            MainFrame.Content = new InschrijvingenOverzicht();
        }

        private void mniBetalingenOverzicht_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BetalingenOverzicht();
        }

        private void mniBetalingToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new BetalingToevoegen();
            window.Owner = this;
            window.ShowDialog();
            MainFrame.Content = new BetalingenOverzicht();
        }

        private void mniGebruikers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gebruikers Beheer - Wordt binnenkort gebouwd");
        }

        private void mniRollen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rollen Beheer - Wordt binnenkort gebouwd");
        }

        private void mniLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Weet je zeker dat je wilt uitloggen?", "Uitloggen", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}