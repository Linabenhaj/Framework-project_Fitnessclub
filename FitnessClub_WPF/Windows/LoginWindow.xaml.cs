using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("Vul email in!", "Fout");
                    return;
                }

                // CORRECTE DB CONTEXT INIT
                var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

                using (var context = new FitnessClubDbContext(optionsBuilder.Options))
                {
                    // Zoek gebruiker
                    var user = context.Users
                        .FirstOrDefault(u => u.Email == EmailTextBox.Text);

                    if (user != null)
                    {
                        var roles = new List<string>();
                        if (!string.IsNullOrEmpty(user.Rol))
                        {
                            roles.Add(user.Rol);
                        }
                        else
                        {
                            roles.Add("Lid");
                        }

                        // Toon welkomstbericht
                        MessageBox.Show($"Welkom {user.Voornaam}!\nRol: {string.Join(", ", roles)}");

                        OpenDashboard(user, roles);
                    }
                    else
                    {
                        MessageBox.Show("Gebruiker niet gevonden! Controleer het emailadres.", "Fout");
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Inlogfout: {ex.Message}", "Fout");
            }
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