using FitnessClub.Models;
using FitnessClub.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
                    // Gebruik Identity voor login
                    var userManager = CreateUserManager(context);
                    var user = await userManager.FindByEmailAsync(EmailTextBox.Text);

                    if (user != null)
                    {
                        // Check wachtwoord via Identity
                        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, PasswordBox.Password);
                        if (isPasswordCorrect)
                        {
                            var roles = (await userManager.GetRolesAsync(user)).ToList();
                            if (roles.Count == 0) roles.Add("Lid");

                            MessageBox.Show($"Welkom {user.Voornaam} ({string.Join(", ", roles)})!");
                            OpenDashboard(user, userManager, roles);
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

        private UserManager<Gebruiker> CreateUserManager(FitnessClubDbContext context)
        {
            var userStore = new UserStore<Gebruiker>(context);
            return new UserManager<Gebruiker>(
                userStore, null, new PasswordHasher<Gebruiker>(), null, null, null, null, null, null);
        }

        private void OpenDashboard(Gebruiker user, UserManager<Gebruiker> userManager, System.Collections.Generic.List<string> roles)
        {
            // alleen roles en user
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