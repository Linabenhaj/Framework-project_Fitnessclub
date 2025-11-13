using FitnessClub.Models;
using FitnessClub.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("Vul email in!", "Fout");
                    return;
                }

                using (var context = new FitnessClubDbContext())
                {
                    // Zoek gebruiker
                    var user = await context.Users
                        .FirstOrDefaultAsync(u => u.Email == EmailTextBox.Text);

                    if (user != null)
                    {
                        // Haal Identity rollen op
                        var userManager = CreateUserManager(context);
                        var identityRoles = await userManager.GetRolesAsync(user);

                        // Combineer rollen
                        var roles = CombineRoles(identityRoles, user.Rol);

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

        private List<string> CombineRoles(IList<string> identityRoles, string customRol)
        {
            var combinedRoles = new List<string>();

            // Voeg Identity rollen toe
            if (identityRoles != null && identityRoles.Count > 0)
            {
                combinedRoles.AddRange(identityRoles);
            }

            // Voeg custom Rol property toe
            if (!string.IsNullOrEmpty(customRol) && !combinedRoles.Contains(customRol))
            {
                combinedRoles.Add(customRol);
            }

          
            if (!combinedRoles.Any())
            {
                combinedRoles.Add("Lid");
            }

            return combinedRoles;
        }

        private UserManager<Gebruiker> CreateUserManager(FitnessClubDbContext context)
        {
            var userStore = new UserStore<Gebruiker>(context);
            return new UserManager<Gebruiker>(
                userStore, null, new PasswordHasher<Gebruiker>(), null, null, null, null, null, null);
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