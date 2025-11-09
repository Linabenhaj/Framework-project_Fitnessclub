using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System;

namespace FitnessClub.WPF.Windows
{
    public partial class BewerkLidWindow : Window
    {
        private readonly string _gebruikerId;
        private readonly FitnessClubDbContext _context;
        private Abonnement _huidigGekozenAbonnement;

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

                    // Huidig abonnement onthouden
                    _huidigGekozenAbonnement = gebruiker.Abonnement;
                    UpdateGekozenAbonnementText();
                }
            }
            catch (System.Exception ex)
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

                AbonnementenPanel.Children.Clear();

                foreach (var abonnement in abonnementen)
                {
                    var border = new Border
                    {
                        Background = System.Windows.Media.Brushes.White,
                        BorderBrush = System.Windows.Media.Brushes.LightGray,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(5),
                        Padding = new Thickness(15),
                        Margin = new Thickness(0, 5, 0, 5),
                        Cursor = System.Windows.Input.Cursors.Hand
                    };

                    var stackPanel = new StackPanel();

                    var naamText = new TextBlock
                    {
                        Text = abonnement.Naam,
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        Foreground = System.Windows.Media.Brushes.DarkSlateBlue
                    };

                    var prijsText = new TextBlock
                    {
                        Text = $"€{abonnement.Prijs:0.00} per maand",
                        FontSize = 14,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = System.Windows.Media.Brushes.ForestGreen
                    };

                    var omschrijvingText = new TextBlock
                    {
                        Text = abonnement.Omschrijving,
                        FontSize = 12,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 5, 0, 0)
                    };

                    stackPanel.Children.Add(naamText);
                    stackPanel.Children.Add(prijsText);
                    stackPanel.Children.Add(omschrijvingText);

                    border.Child = stackPanel;

                    // Click event voor abonnement selectie
                    border.MouseLeftButtonDown += (s, e) =>
                    {
                        _huidigGekozenAbonnement = abonnement;
                        UpdateGekozenAbonnementText();

                      


                        foreach (var child in AbonnementenPanel.Children)
                        {
                            if (child is Border otherBorder)
                            {
                                otherBorder.BorderBrush = System.Windows.Media.Brushes.LightGray;
                                otherBorder.Background = System.Windows.Media.Brushes.White;
                            }
                        }

                        border.BorderBrush = System.Windows.Media.Brushes.Green;
                        border.Background = System.Windows.Media.Brushes.LightGreen;
                    };

                    // Markeer huidig geselecteerd abonnement


                    if (_huidigGekozenAbonnement != null && _huidigGekozenAbonnement.Id == abonnement.Id)
                    {
                        border.BorderBrush = System.Windows.Media.Brushes.Green;
                        border.Background = System.Windows.Media.Brushes.LightGreen;
                    }

                    AbonnementenPanel.Children.Add(border);
                }

                // Voeg "Geen abonnement" optie toe
                var geenAbonnementBorder = new Border
                {
                    Background = System.Windows.Media.Brushes.White,
                    BorderBrush = _huidigGekozenAbonnement == null ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 5, 0, 5),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var geenAbonnementStack = new StackPanel();
                var geenAbonnementText = new TextBlock
                {
                    Text = "Geen abonnement",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    FontStyle = FontStyles.Italic
                };

                geenAbonnementStack.Children.Add(geenAbonnementText);
                geenAbonnementBorder.Child = geenAbonnementStack;

                geenAbonnementBorder.MouseLeftButtonDown += (s, e) =>
                {
                    _huidigGekozenAbonnement = null;
                    UpdateGekozenAbonnementText();

                    foreach (var child in AbonnementenPanel.Children)
                    {
                        if (child is Border otherBorder)
                        {
                            otherBorder.BorderBrush = System.Windows.Media.Brushes.LightGray;
                            otherBorder.Background = System.Windows.Media.Brushes.White;
                        }
                    }

                    geenAbonnementBorder.BorderBrush = System.Windows.Media.Brushes.Green;
                    geenAbonnementBorder.Background = System.Windows.Media.Brushes.LightGreen;
                };

                AbonnementenPanel.Children.Insert(0, geenAbonnementBorder);

            }
            catch (System.Exception ex)
            {
                ValidatieText.Text = $"Fout bij laden abonnementen: {ex.Message}";
            }
        }

        private void UpdateGekozenAbonnementText()
        {
            if (_huidigGekozenAbonnement != null)
            {
                GekozenAbonnementText.Text = $"Geselecteerd: {_huidigGekozenAbonnement.Naam} - €{_huidigGekozenAbonnement.Prijs:0.00}/maand";
            }
            else
            {
                GekozenAbonnementText.Text = "Geselecteerd: Geen abonnement";
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
                var gebruiker = _context.Users
                    .Include(u => u.Abonnement)
                    .FirstOrDefault(u => u.Id == _gebruikerId);

                if (gebruiker != null)
                {
                    gebruiker.Voornaam = VoornaamTextBox.Text.Trim();
                    gebruiker.Achternaam = AchternaamTextBox.Text.Trim();
                    gebruiker.Email = EmailTextBox.Text.Trim();
                    gebruiker.Telefoon = TelefoonTextBox.Text.Trim();
                    gebruiker.Geboortedatum = GeboortedatumPicker.SelectedDate.Value;

                    if (_huidigGekozenAbonnement != null)
                    {
                        gebruiker.AbonnementId = _huidigGekozenAbonnement.Id;
                    }
                    else
                    {
                        gebruiker.AbonnementId = null;
                    }

                    _context.SaveChanges();
                    this.DialogResult = true;
                    this.Close();
                }


            }
            catch (System.Exception ex)
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
                VoornaamTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(AchternaamTextBox.Text))
            {
                ValidatieText.Text = "Achternaam is verplicht!";
                AchternaamTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ValidatieText.Text = "E-mail is verplicht!";
                EmailTextBox.Focus();
                return false;
            }

            if (GeboortedatumPicker.SelectedDate == null)
            {
                ValidatieText.Text = "Geboortedatum is verplicht!";
                GeboortedatumPicker.Focus();
                return false;
            }

            if (GeboortedatumPicker.SelectedDate.Value > DateTime.Now.AddYears(-16))
            {
                ValidatieText.Text = "Je moet minimaal 16 jaar oud zijn!";
                GeboortedatumPicker.Focus();
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                ValidatieText.Text = "Voer een geldig e-mailadres in!";
                EmailTextBox.Focus();
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
    }
}