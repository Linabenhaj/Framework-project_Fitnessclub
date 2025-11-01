using System.Windows;

namespace FitnessClub.WPF
{
    public partial class LoginFormWindow : Window
    {
        public LoginFormWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // test login
            this.DialogResult = true;
            this.Close();
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}