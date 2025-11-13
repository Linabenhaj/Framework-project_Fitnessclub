using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FitnessClub.WPF.Windows
{
    public partial class BewerkLidWindow : Window
    {
        private readonly string _gebruikerId;
        private readonly FitnessClubDbContext _context;
        private Gebruiker _gebruiker;

        public BewerkLidWindow(string gebruikerId)
        {
            InitializeComponent();
            _gebruikerId = gebruikerId;
            _context = new FitnessClubDbContext();

            LaadGebruikerData();
            LaadAbonnementen();
            VulRollenComboBox();
        }

        private void LaadGebruikerData()
        {
            try
            {
                _gebruiker = _context.Users
                    .Include(u => u.Abonnement)
                    .FirstOrDefault(u => u.Id == _gebruikerId);

                if (_gebruiker != null)
                {
                    VoornaamTextBox.Text = _gebruiker.Voornaam;
                    AchternaamTextBox.Text = _gebruiker.Achternaam;
                    EmailTextBox.Text = _gebruiker.Email;
                    TelefoonTextBox.Text = _gebruiker.Telefoon ?? "";
                    GeboortedatumPicker.SelectedDate = _gebruiker.Geboortedatum;

                    // Toon huidige rol
                    HuidigeRolText.Text = $"Huidige rol: {_gebruiker.Rol ?? "Lid"}";

                    // Selecteer huidige rol in combobox
                    if (!string.IsNullOrEmpty(_gebruiker.Rol))
                    {
                        RolComboBox.SelectedItem = _gebruiker.Rol;
                    }

                    // Toon blokkeer status
                    IsGeblokkeerdCheckBox.IsChecked = _gebruiker.IsVerwijderd;
                    UpdateBlokkeerStatusText();

                    // Stel huidig abonnement in
                    if (_gebruiker.Abonnement != null)
                    {
                        AbonnementenComboBox.SelectedValue = _gebruiker.Abonnement.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                ValidatieText.Text = $"Fout bij laden gegevens: {ex.Message}";
            }
        }

        private void VulRollenComboBox()
        {
            // Vul rollen combobox
            RolComboBox.Items.Add("Lid");
            RolComboBox.Items.Add("Admin");
            RolComboBox.SelectedIndex = 0; // Standaard "Lid"
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

        private void UpdateBlokkeerStatusText()
        {
            if (_gebruiker != null)
            {
                if (_gebruiker.IsVerwijderd)
                {
                    BlokkeerStatusText.Text = "Status: GEBLOKKEERD (kan niet inloggen)";
                    BlokkeerStatusText.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    BlokkeerStatusText.Text = "Status: Actief (kan inloggen)";
                    BlokkeerStatusText.Foreground = System.Windows.Media.Brushes.Green;
                }
            }
        }

        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            if (!ValideerFormulier())
                return;

            try
            {
                if (_gebruiker != null)
                {
                    var oudeEmail = _gebruiker.Email;
                    var nieuweEmail = EmailTextBox.Text.Trim();

                    // Update basisgegevens
                    _gebruiker.Voornaam = VoornaamTextBox.Text.Trim();
                    _gebruiker.Achternaam = AchternaamTextBox.Text.Trim();
                    _gebruiker.Email = nieuweEmail;
                    _gebruiker.Telefoon = TelefoonTextBox.Text.Trim();
                    _gebruiker.Geboortedatum = GeboortedatumPicker.SelectedDate.Value;

                    // Update ook de genormaliseerde email voor Identity
                    if (oudeEmail != nieuweEmail)
                    {
                        _gebruiker.NormalizedEmail = nieuweEmail.ToUpper();
                        _gebruiker.NormalizedUserName = nieuweEmail.ToUpper();
                    }

                    // Update abonnement
                    var geselecteerd = AbonnementenComboBox.SelectedItem as AbonnementKeuze;
                    _gebruiker.AbonnementId = geselecteerd?.Id;

                    // Rol bijwerken als deze gewijzigd is
                    var nieuweRol = RolComboBox.SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(nieuweRol) && nieuweRol != _gebruiker.Rol)
                    {
                        await UpdateGebruikerRol(_gebruiker, nieuweRol);
                    }

                    // Blokkeer status bijwerken
                    var wasGeblokkeerd = _gebruiker.IsVerwijderd;
                    _gebruiker.IsVerwijderd = IsGeblokkeerdCheckBox.IsChecked ?? false;

                    if (_gebruiker.IsVerwijderd && !wasGeblokkeerd)
                    {
                        _gebruiker.VerwijderdOp = DateTime.Now;
                    }
                    else if (!_gebruiker.IsVerwijderd && wasGeblokkeerd)
                    {
                        _gebruiker.VerwijderdOp = null;
                    }

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

        private async System.Threading.Tasks.Task UpdateGebruikerRol(Gebruiker gebruiker, string nieuweRol)
        {
            try
            {
                var userManager = CreateUserManager();

                // Verwijder uit alle bestaande rollen
                var huidigeRoles = await userManager.GetRolesAsync(gebruiker);
                if (huidigeRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(gebruiker, huidigeRoles);
                }

                // Voeg toe aan nieuwe rol
                await userManager.AddToRoleAsync(gebruiker, nieuweRol);

                // Update ook de custom Rol property
                gebruiker.Rol = nieuweRol;

                MessageBox.Show($"Rol gewijzigd naar: {nieuweRol}", "Rol bijgewerkt");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij wijzigen rol: {ex.Message}\n" +
                              $"De basisgegevens zijn opgeslagen, maar de rol moet handmatig worden aangepast.", "Waarschuwing");
            }
        }

        private UserManager<Gebruiker> CreateUserManager()
        {
            var userStore = new UserStore<Gebruiker>(_context);
            return new UserManager<Gebruiker>(
                userStore, null, new PasswordHasher<Gebruiker>(), null, null, null, null, null, null);
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
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

        private void IsGeblokkeerdCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateBlokkeerStatusText();
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