using System.Windows;

namespace FitnessClub.WPF
{
    public partial class LoginWindow : Window
    {
        private readonly string _rol;

        public LoginWindow(string rol = "")
        {
            InitializeComponent();
            _rol = rol;
            Title = $"Inloggen als {rol} - Fitness Club";
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // DEMO: Simpele login voor testing
            if (_rol == "Admin" && EmailTextBox.Text == "admin@fitness.com" && PasswordBox.Password == "Admin123!")
            {
                var dashboard = new DashboardWindow(null, new System.Collections.Generic.List<string> { "Admin" });
                dashboard.Show();
                this.Close();
            }
            else if (_rol == "Lid")
            {
                // Voor demo - accepteer elke login als lid
                var dashboard = new DashboardWindow(null, new System.Collections.Generic.List<string> { "Lid" });
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ongeldige inloggegevens! Gebruik admin@fitness.com / Admin123! voor Admin");
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