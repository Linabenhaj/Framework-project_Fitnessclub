using System.Windows;

namespace FitnessClub.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            // Toon admin login scherm
            var loginWindow = new LoginWindow("Admin");
            loginWindow.Show();
            this.Hide();
        }

        private void LidLogin_Click(object sender, RoutedEventArgs e)
        {
            // Toon lid login scherm
            var loginWindow = new LoginWindow("Lid");
            loginWindow.Show();
            this.Hide();
        }

        private void Registreer_Click(object sender, RoutedEventArgs e)
        {
            
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Hide();
        }
    }
}