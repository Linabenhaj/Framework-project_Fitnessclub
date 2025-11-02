using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Registreer_Click(object sender, RoutedEventArgs e)
        {
            // Valideer input
            if (string.IsNullOrWhiteSpace(VoornaamTextBox.Text) ||
                string.IsNullOrWhiteSpace(AchternaamTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Vul alle velden in!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Wachtwoorden komen niet overeen!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            MessageBox.Show("Registratie succesvol! (Demo)", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

            // Terug naar login
            var loginWindow = new LoginWindow("Lid");
            loginWindow.Show();
            this.Close();
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            // Terug naar hoofdmenu
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}