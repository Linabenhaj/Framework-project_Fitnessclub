using System.Windows;

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
            try
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

                if (GeboortedatumPicker.SelectedDate == null)
                {
                    MessageBox.Show("Selecteer een geboortedatum!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // DEMO: Registratie succesvol
                MessageBox.Show("Registratie succesvol! (Demo - gebruiker wordt niet opgeslagen)", "Succes",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                // Terug naar login
                var loginWindow = new LoginWindow("Lid");
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registratiefout: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}