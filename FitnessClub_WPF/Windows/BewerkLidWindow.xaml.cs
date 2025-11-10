using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FitnessClub.WPF.Windows
{
    public partial class BewerkLidWindow : Window
    {
        private readonly string _gebruikerId;
        private readonly FitnessClubDbContext _context;

        public BewerkLidWindow(string gebruikerId)
        {
            InitializeComponent();
            _gebruikerId = gebruikerId;
            _context = new FitnessClubDbContext();

            LaadGebruikerData();
            LaadAbonnementen();
        }

        private void LaadGebruikerData()
        {
            try
            {
                var gebruiker = _context.Users
                    .Include(u => u.Abonnement)
                    .FirstOrDefault(u => u.Id == _gebruikerId);

                if (gebruiker != null)
                {
                    VoornaamTextBox.Text = gebruiker.Voornaam;
                    AchternaamTextBox.Text = gebruiker.Achternaam;
                    EmailTextBox.Text = gebruiker.Email;
                    TelefoonTextBox.Text = gebruiker.Telefoon ?? "";
                    GeboortedatumPicker.SelectedDate = gebruiker.Geboortedatum;

                    // Stel huidig abonnement in
                    if (gebruiker.Abonnement != null)
                    {
                        AbonnementenComboBox.SelectedValue = gebruiker.Abonnement.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                ValidatieText.Text = $"Fout bij laden gegevens: {ex.Message}";
            }
        }

        private void LaadAbonnementen()
        {
            try
            {
                var abonnementen = _context.Abonnementen
                    .Where(a => !a.IsVerwijderd)
                    .OrderBy(a => a.Prijs)
                    .ToList();

                // Voeg "Geen abonnement" optie toe
                var keuzes = new List<AbonnementKeuze>
                {
                    new AbonnementKeuze { Id = null, Naam = "Geen abonnement", Prijs = 0 }
                };

                keuzes.AddRange(abonnementen.Select(a => new AbonnementKeuze
                {
                    Id = a.Id,
                    Naam = a.Naam,
                    Prijs = a.Prijs,
                    Omschrijving = a.Omschrijving
                }));

                AbonnementenComboBox.ItemsSource = keuzes;
                AbonnementenComboBox.DisplayMemberPath = "DisplayText";
                AbonnementenComboBox.SelectedValuePath = "Id";

                UpdateGekozenAbonnementText();
            }
            catch (Exception ex)
            {
                ValidatieText.Text = $"Fout bij laden abonnementen: {ex.Message}";
            }
        }

        private void UpdateGekozenAbonnementText()
        {
            var geselecteerd = AbonnementenComboBox.SelectedItem as AbonnementKeuze;
            if (geselecteerd != null)
            {
                if (geselecteerd.Id == null)
                {
                    GekozenAbonnementText.Text = "Geselecteerd: Geen abonnement";
                }
                else
                {
                    GekozenAbonnementText.Text = $"Geselecteerd: {geselecteerd.Naam} - €{geselecteerd.Prijs:0.00}/maand";
                }
            }
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            if (!ValideerFormulier())
                return;

            try
            {
                var gebruiker = _context.Users.FirstOrDefault(u => u.Id == _gebruikerId);

                if (gebruiker != null)
                {
                    var oudeEmail = gebruiker.Email;
                    var nieuweEmail = EmailTextBox.Text.Trim();

                    // Update basisgegevens
                    gebruiker.Voornaam = VoornaamTextBox.Text.Trim();
                    gebruiker.Achternaam = AchternaamTextBox.Text.Trim();
                    gebruiker.Email = nieuweEmail;
                    gebruiker.Telefoon = TelefoonTextBox.Text.Trim();
                    gebruiker.Geboortedatum = GeboortedatumPicker.SelectedDate.Value;

                    // BELANGRIJK: Update ook de genormaliseerde email voor Identity
                    if (oudeEmail != nieuweEmail)
                    {
                        gebruiker.NormalizedEmail = nieuweEmail.ToUpper();
                        gebruiker.NormalizedUserName = nieuweEmail.ToUpper();
                    }

                    // Update abonnement
                    var geselecteerd = AbonnementenComboBox.SelectedItem as AbonnementKeuze;
                    gebruiker.AbonnementId = geselecteerd?.Id;

                    _context.SaveChanges();

                    MessageBox.Show("Gebruiker succesvol bijgewerkt!", "Succes");
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ValidatieText.Text = $"Fout bij opslaan: {ex.Message}";
            }
        }

        private bool ValideerFormulier()
        {
            ValidatieText.Text = "";

            if (string.IsNullOrWhiteSpace(VoornaamTextBox.Text))
            {
                ValidatieText.Text = "Voornaam is verplicht!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(AchternaamTextBox.Text))
            {
                ValidatieText.Text = "Achternaam is verplicht!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ValidatieText.Text = "E-mail is verplicht!";
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                ValidatieText.Text = "Voer een geldig e-mailadres in!";
                return false;
            }

            // Controleer of email al bestaat bij een andere gebruiker
            try
            {
                var nieuweEmail = EmailTextBox.Text.Trim();
                var bestaandeGebruiker = _context.Users
                    .FirstOrDefault(u => u.Email == nieuweEmail && u.Id != _gebruikerId);

                if (bestaandeGebruiker != null)
                {
                    ValidatieText.Text = "Dit e-mailadres is al in gebruik door een andere gebruiker!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ValidatieText.Text = $"Fout bij controle e-mail: {ex.Message}";
                return false;
            }

            if (GeboortedatumPicker.SelectedDate == null)
            {
                ValidatieText.Text = "Geboortedatum is verplicht!";
                return false;
            }

            if (GeboortedatumPicker.SelectedDate.Value > DateTime.Now.AddYears(-16))
            {
                ValidatieText.Text = "Je moet minimaal 16 jaar oud zijn!";
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void AbonnementenComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGekozenAbonnementText();
        }

        // Hulpklasse voor abonnement keuzes
        private class AbonnementKeuze
        {
            public int? Id { get; set; }
            public string Naam { get; set; }
            public decimal Prijs { get; set; }
            public string Omschrijving { get; set; }
            public string DisplayText => Id == null ? "Geen abonnement" : $"{Naam} - €{Prijs:0.00}/maand";
        }
    }
}