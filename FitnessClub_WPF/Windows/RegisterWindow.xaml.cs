using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using System.Collections.Generic;

namespace FitnessClub.WPF
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Registreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(VoornaamTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AchternaamTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Vul alle verplichte velden in!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("Wachtwoorden komen niet overeen!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (GeboortedatumPicker.SelectedDate == null)
                {
                    MessageBox.Show("Selecteer een geboortedatum!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Controleer of gebruiker al bestaat
                using (var context = new FitnessClubDbContext())
                {
                    var existingUser = context.Users.FirstOrDefault(u => u.Email == EmailTextBox.Text);
                    if (existingUser != null)
                    {
                        MessageBox.Show("Er bestaat al een gebruiker met dit e-mailadres!", "Fout",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Maak nieuwe gebruiker aan
                    var user = new Gebruiker
                    {
                        Voornaam = VoornaamTextBox.Text,
                        Achternaam = AchternaamTextBox.Text,
                        Email = EmailTextBox.Text,
                        UserName = EmailTextBox.Text,
                        Telefoon = TelefoonTextBox.Text,
                        Geboortedatum = GeboortedatumPicker.SelectedDate.Value
                    };

                    context.Users.Add(user);
                    context.SaveChanges();

                    MessageBox.Show("Registratie succesvol! Gebruiker opgeslagen in database.", "Succes",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    // Ga naar dashboard als Lid
                    var dashboard = new DashboardWindow(new List<string> { "Lid" }, user);
                    dashboard.Show();
                    this.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Registratiefout: {ex.Message}", "Fout",
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