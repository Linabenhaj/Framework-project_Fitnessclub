using FitnessClub.WPF.Views;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //  welkom pagina bij startup
            MainFrame.Content = new WelcomePage();
        }

        //  navigation 
        private void mniLedenOverzicht_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new LedenOverzicht();
        }

        private void mniLidToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new LidToevoegenWindow();
            window.Owner = this;
            window.ShowDialog();
            // Refresh na toevoegen
            MainFrame.Content = new LedenOverzicht();
        }

        private void mniAbonnementenOverzicht_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AbonnementenOverzicht();
        }

        private void mniAbonnementToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new AbonnementToevoegenWindow();
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
            var window = new InschrijvingToevoegenWindow();
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
            var window = new BetalingToevoegenWindow();
            window.Owner = this;
            window.ShowDialog();
            MainFrame.Content = new BetalingenOverzicht();
        }

        private void mniLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }

    // welcome pagina
    public class WelcomePage : System.Windows.Controls.Page
    {
        public WelcomePage()
        {
            var textBlock = new System.Windows.Controls.TextBlock
            {
                Text = "Welkom bij Fitness Club Management\nKies een optie uit het menu om te beginnen.",
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            this.Content = textBlock;
        }
    }
}