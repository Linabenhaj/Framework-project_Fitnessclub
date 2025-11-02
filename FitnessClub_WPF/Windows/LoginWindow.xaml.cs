using System.Windows;
using Microsoft.AspNetCore.Identity;
using FitnessClub.Models.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace FitnessClub.WPF
{
    public partial class LoginWindow : Window
    {
        private string _loginType;
        private readonly UserManager<Gebruiker> _userManager;
        private readonly SignInManager<Gebruiker> _signInManager;

        public LoginWindow(string loginType)
        {
            InitializeComponent();

            _loginType = loginType;
            _userManager = App.ServiceProvider.GetRequiredService<UserManager<Gebruiker>>();
            _signInManager = App.ServiceProvider.GetRequiredService<SignInManager<Gebruiker>>();

            TitleText.Text = $"{loginType} Inloggen";
            this.Title = $"{loginType} Inloggen - Fitness Club";
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = EmailTextBox.Text;
                string password = PasswordBox.Password;

                // Validatie
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Vul email en wachtwoord in!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Inloggen
                var result = await _signInManager.PasswordSignInAsync(
                    email, password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                   
                    var user = await _userManager.FindByEmailAsync(email);
                    var roles = await _userManager.GetRolesAsync(user);

                    // Controleer op login 
                    if (_loginType == "Admin" && !roles.Contains("Admin"))
                    {
                        MessageBox.Show("Je hebt geen admin rechten!", "Toegang geweigerd",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        await _signInManager.SignOutAsync();
                        return;
                    }

                    if (_loginType == "Lid" && !roles.Contains("Lid"))
                    {
                        MessageBox.Show("Je bent niet geregistreerd als lid!", "Toegang geweigerd",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        await _signInManager.SignOutAsync();
                        return;
                    }

                    // Dashboard
                    var dashboard = new DashboardWindow(user, roles.ToList());
                    dashboard.Show();
                    this.Close();
                }
                else if (result.IsLockedOut)
                {
                    MessageBox.Show("Account is geblokkeerd!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (result.IsNotAllowed)
                {
                    MessageBox.Show("Login niet toegestaan. Controleer je email verificatie.", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Ongeldige email of wachtwoord!", "Login mislukt",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er ging iets mis tijdens het inloggen: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            // Terug naar hoofdmenu
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}