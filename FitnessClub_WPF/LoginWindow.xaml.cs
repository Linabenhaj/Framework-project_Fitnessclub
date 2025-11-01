using System.Windows;

namespace FitnessClub.WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

        }



        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // test authentication 
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }
    }
}