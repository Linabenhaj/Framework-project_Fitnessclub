using FitnessClub.Models;
using FitnessClub.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF
{
    public partial class RegisterWindow : Window
    {
        private List<Abonnement> _beschikbareAbonnementen;
        private Abonnement _geselecteerdAbonnement;

        public RegisterWindow()
        {
            InitializeComponent();
            LaadAbonnementen();
        }

        private void LaadAbonnementen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    _beschikbareAbonnementen = context.Abonnementen
                        .Where(a => !a.IsVerwijderd)
                        .OrderBy(a => a.Prijs)
                        .ToList();

                    ToonAbonnementKeuzes();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}", "Fout");
            }
        }

        private void ToonAbonnementKeuzes()
        {
            AbonnementenPanel.Children.Clear();

            foreach (var abonnement in _beschikbareAbonnementen)
            {
                var radioButton = new RadioButton
                {
                    Content = $"{abonnement.Naam} - €{abonnement.Prijs:0.00}/maand",
                    Tag = abonnement,
                    Margin = new Thickness(0, 5, 0, 5),
                    FontSize = 14
                };

                radioButton.Checked += (s, e) =>
                {
                    _geselecteerdAbonnement = (Abonnement)radioButton.Tag;
                    GekozenAbonnementText.Text = $"Gekozen: {_geselecteerdAbonnement.Naam}";
                };

                AbonnementenPanel.Children.Add(radioButton);
            }

            if (_beschikbareAbonnementen.Any())
            {
                var eersteRadioButton = AbonnementenPanel.Children[0] as RadioButton;
                if (eersteRadioButton != null)
                {
                    eersteRadioButton.IsChecked = true;
                }
            }
        }

        private async void Registreer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                

                using (var context = new FitnessClubDbContext())
                {
                    var userManager = CreateUserManager(context);

                    // Controleer of gebruiker al bestaat
                    var existingUser = await userManager.FindByEmailAsync(EmailTextBox.Text);
                    if (existingUser != null)
                    {
                        MessageBox.Show("Er bestaat al een gebruiker met dit e-mailadres!", "Fout");
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
                        Geboortedatum = GeboortedatumPicker.SelectedDate.Value,
                        AbonnementId = _geselecteerdAbonnement.Id
                    };

                    // GEBRUIK IDENTITY VOOR REGISTRATIE
                    var result = await userManager.CreateAsync(user, PasswordBox.Password);

                    if (result.Succeeded)
                    {
                        // Voeg toe aan Lid rol
                        await userManager.AddToRoleAsync(user, "Lid");

                        MessageBox.Show($"Registratie succesvol! Je kan nu inloggen.", "Succes");

                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        var errors = string.Join("\n", result.Errors.Select(err => err.Description));
                        MessageBox.Show($"Registratiefout: {errors}", "Fout");
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Registratiefout: {ex.Message}", "Fout");
            }
        }

        private UserManager<Gebruiker> CreateUserManager(FitnessClubDbContext context)
        {
            var userStore = new UserStore<Gebruiker>(context);
            return new UserManager<Gebruiker>(
                userStore, null, new PasswordHasher<Gebruiker>(), null, null, null, null, null, null);
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}