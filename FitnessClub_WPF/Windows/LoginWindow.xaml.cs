using FitnessClub.Models;
using FitnessClub.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Vul alle velden in!", "Fout");
                    return;
                }

                using (var context = new FitnessClubDbContext())
                {
                    // Eenvoudige login
                    var user = await context.Users
                        .FirstOrDefaultAsync(u => u.Email == EmailTextBox.Text);

                    if (user != null)
                    {
                        // Controleer welk wachtwoordveld beschikbaar is
                        var passwordToCheck = GetPasswordFromUser(user);

                        if (passwordToCheck == PasswordBox.Password)
                        {
                            var roles = new List<string> { user.Rol ?? "Lid" };
                            MessageBox.Show($"Welkom {user.Voornaam} ({string.Join(", ", roles)})!");
                            OpenDashboard(user, roles);
                        }
                        else
                        {
                            MessageBox.Show("Ongeldig wachtwoord!", "Fout");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Gebruiker niet gevonden!", "Fout");
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Inlogfout: {ex.Message}", "Fout");
            }
        }

        //  om wachtwoord te vinden
        private string GetPasswordFromUser(Gebruiker user)
        {
            // Probeer verschillende mogelijke veldnamen
            var userType = user.GetType();

            // Controleer verschillende mogelijke wachtwoord veldnamen
            var possiblePasswordFields = new[] { "PasswordHash", "Wachtwoord", "Password", "Passwoord", "WachtwoordHash" };

            foreach (var fieldName in possiblePasswordFields)
            {
                var property = userType.GetProperty(fieldName);
                if (property != null)
                {
                    var value = property.GetValue(user) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return string.Empty;
        }

        private void OpenDashboard(Gebruiker user, List<string> roles)
        {
            var dashboard = new DashboardWindow(roles, user);
            dashboard.Show();
            this.Close();
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Registreer_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}