using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class RegisterWindow : Window
    {
        private readonly FitnessClubDbContext _context;

        public RegisterWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);

            VulAbonnementen();
        }

        private void VulAbonnementen()
        {
            try
            {
                var abonnementen = _context.Abonnementen
                    .Where(a => a.IsActief)
                    .ToList();

                AbonnementenPanel.Children.Clear();

                foreach (var abonnement in abonnementen)
                {
                    var radioButton = new System.Windows.Controls.RadioButton
                    {
                        Content = $"{abonnement.Naam} - €{abonnement.Prijs:F2}/maand",
                        Tag = abonnement.Id,
                        Margin = new Thickness(0, 5, 0, 5),
                        FontSize = 14
                    };
                    radioButton.Checked += (s, e) =>
                    {
                        GekozenAbonnementText.Text = $"Gekozen: {abonnement.Naam}";
                    };

                    AbonnementenPanel.Children.Add(radioButton);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegistreerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValideerInvoer())
                    return;

                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("Wachtwoorden komen niet overeen", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Zoek geselecteerd abonnement
                int? abonnementId = null;
                foreach (var child in AbonnementenPanel.Children)
                {
                    if (child is System.Windows.Controls.RadioButton radioButton && radioButton.IsChecked == true)
                    {
                        abonnementId = (int)radioButton.Tag;
                        break;
                    }
                }

                var nieuwLid = new Gebruiker
                {
                    Voornaam = VoornaamTextBox.Text,
                    Achternaam = AchternaamTextBox.Text,
                    Email = EmailTextBox.Text,
                    UserName = EmailTextBox.Text,
                    PhoneNumber = TelefoonTextBox.Text,
                    Geboortedatum = GeboortedatumPicker.SelectedDate.Value,
                    Rol = "Lid",
                    AbonnementId = abonnementId,
                    EmailConfirmed = true
                };

                // Note: In een echte app zou je Identity gebruiken voor wachtwoorden
                // Voor nu gebruiken we een simpele hash
                nieuwLid.PasswordHash = HashWachtwoord(PasswordBox.Password);

                _context.Users.Add(nieuwLid);  // Gebruik Users, niet Gebruikers
                _context.SaveChanges();

                MessageBox.Show("Registratie succesvol!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij registratie: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValideerInvoer()
        {
            if (string.IsNullOrWhiteSpace(VoornaamTextBox.Text))
            {
                MessageBox.Show("Voer een voornaam in", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(AchternaamTextBox.Text))
            {
                MessageBox.Show("Voer een achternaam in", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !EmailTextBox.Text.Contains("@"))
            {
                MessageBox.Show("Voer een geldig emailadres in", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password) || PasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Wachtwoord moet minimaal 6 tekens zijn", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (GeboortedatumPicker.SelectedDate == null)
            {
                MessageBox.Show("Selecteer een geboortedatum", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Controleer of er een abonnement is geselecteerd
            bool abonnementGeselecteerd = false;
            foreach (var child in AbonnementenPanel.Children)
            {
                if (child is System.Windows.Controls.RadioButton radioButton && radioButton.IsChecked == true)
                {
                    abonnementGeselecteerd = true;
                    break;
                }
            }

            if (!abonnementGeselecteerd)
            {
                MessageBox.Show("Selecteer een abonnement", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private string HashWachtwoord(string wachtwoord)
        {
            // SIMPELE HASH - IN PRODUCTIE: gebruik PasswordHasher van Identity
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(wachtwoord);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}