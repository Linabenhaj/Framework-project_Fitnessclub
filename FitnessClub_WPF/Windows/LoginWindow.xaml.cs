using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using System.Collections.Generic;

namespace FitnessClub.WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Vul alle velden in!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var context = new FitnessClubDbContext())
                {
                    // Zoek gebruiker in database
                    var user = context.Users
                        .FirstOrDefault(u => u.Email == EmailTextBox.Text);

                    if (user != null)
                    {
                        // DEMO: Simpele login voor testing
                       
                        if (EmailTextBox.Text == "admin@fitness.com" && PasswordBox.Password == "Admin123!")
                        {
                            var dashboard = new DashboardWindow(new List<string> { "Admin" }, user);
                            dashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            // Voor demo  accepteer elke login als lid
                            var dashboard = new DashboardWindow(new List<string> { "Lid" }, user);
                            dashboard.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Gebruiker niet gevonden! Registreer eerst.", "Fout",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Inlogfout: {ex.Message}", "Fout",
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