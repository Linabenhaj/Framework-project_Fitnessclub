using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace FitnessClub.WPF.Views
{
    public partial class MijnAbonnement : UserControl
    {
        private Gebruiker _huidigeGebruiker;
        private Abonnement _geselecteerdAbonnement;

        public MijnAbonnement()
        {
            InitializeComponent();
            Loaded += MijnAbonnement_Loaded;
        }

        private void MijnAbonnement_Loaded(object sender, RoutedEventArgs e)
        {
            LaadMijnAbonnement();
            LaadBeschikbareAbonnementen();
        }

        private void LaadMijnAbonnement()
        {
            try
            {
                StatusText.Text = "Laden...";

                using (var context = new FitnessClubDbContext())
                {
                    // Gebruik eerste gebruiker (in echte app: ingelogde gebruiker)
                    _huidigeGebruiker = context.Users
                        .Include(u => u.Abonnement)
                        .FirstOrDefault();

                    if (_huidigeGebruiker?.Abonnement != null)
                    {
                        AbonnementNaamText.Text = _huidigeGebruiker.Abonnement.Naam;
                        AbonnementPrijsText.Text = $"Prijs: €{_huidigeGebruiker.Abonnement.Prijs:0.00} per maand";
                        AbonnementOmschrijvingText.Text = _huidigeGebruiker.Abonnement.Omschrijving;
                        AbonnementStatusText.Text = "✅ Actief abonnement";
                        AbonnementStatusText.Foreground = System.Windows.Media.Brushes.Green;
                    }
                    else
                    {
                        AbonnementNaamText.Text = "Geen abonnement";
                        AbonnementPrijsText.Text = "U heeft momenteel geen actief abonnement";
                        AbonnementOmschrijvingText.Text = "Selecteer een abonnement hieronder om te beginnen";
                        AbonnementStatusText.Text = "❌ Geen actief abonnement";
                        AbonnementStatusText.Foreground = System.Windows.Media.Brushes.Red;
                    }
                }

                StatusText.Text = "Geladen";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Fout bij laden";
                MessageBox.Show($"Fout bij laden abonnement: {ex.Message}", "Fout");

                // Fallback values
                AbonnementNaamText.Text = "Geen abonnement";
                AbonnementPrijsText.Text = "Fout bij laden gegevens";
                AbonnementOmschrijvingText.Text = "Probeer opnieuw of neem contact op met support";
                AbonnementStatusText.Text = "❌ Fout bij laden";
                AbonnementStatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void LaadBeschikbareAbonnementen()
        {
            try
            {
                StatusText.Text = "Laden abonnementen...";

                using (var context = new FitnessClubDbContext())
                {
                    var alleAbonnementen = context.Abonnementen
                        .Where(a => !a.IsVerwijderd)
                        .OrderBy(a => a.Prijs)
                        .ToList();

                    if (alleAbonnementen.Any())
                    {
                        AbonnementenListView.ItemsSource = alleAbonnementen;

                        // Selecteer automatisch eerste abonnement als er geen huidig is
                        if (_huidigeGebruiker?.AbonnementId == null && alleAbonnementen.Any())
                        {
                            AbonnementenListView.SelectedItem = alleAbonnementen.First();
                            _geselecteerdAbonnement = alleAbonnementen.First();
                            UpdateWijzigButton();
                        }
                        // Selecteer huidig abonnement in de lijst
                        else if (_huidigeGebruiker?.AbonnementId != null)
                        {
                            var huidigAbonnement = alleAbonnementen.FirstOrDefault(a => a.Id == _huidigeGebruiker.AbonnementId);
                            if (huidigAbonnement != null)
                            {
                                AbonnementenListView.SelectedItem = huidigAbonnement;
                                _geselecteerdAbonnement = huidigAbonnement;
                                UpdateWijzigButton();
                            }
                        }
                    }
                    else
                    {
                        StatusText.Text = "Geen abonnementen beschikbaar";
                    }
                }

                StatusText.Text = "Klaar";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Fout bij laden abonnementen";
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}", "Fout");
            }
        }

        // 👇 DEZE METHODE WAS MISSENDE - NU TOEGEVOEGD
        private void AbonnementenListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AbonnementenListView.SelectedItem is Abonnement selected)
            {
                _geselecteerdAbonnement = selected;
                UpdateWijzigButton();

                // Debug info
                StatusText.Text = $"Geselecteerd: {selected.Naam}";
            }
            else
            {
                StatusText.Text = "Geen abonnement geselecteerd";
            }
        }

        private void UpdateWijzigButton()
        {
            if (_geselecteerdAbonnement != null && _huidigeGebruiker != null)
            {
                bool isZelfdeAbonnement = _huidigeGebruiker.AbonnementId == _geselecteerdAbonnement.Id;

                if (isZelfdeAbonnement)
                {
                    WijzigButton.Content = "Huidig Abonnement";
                    WijzigButton.Background = System.Windows.Media.Brushes.Gray;
                    WijzigButton.IsEnabled = false;
                    StatusText.Text = $"Huidig abonnement: {_geselecteerdAbonnement.Naam}";
                }
                else
                {
                    WijzigButton.Content = $"Wijzig naar {_geselecteerdAbonnement.Naam}";
                    WijzigButton.Background = System.Windows.Media.Brushes.DodgerBlue;
                    WijzigButton.IsEnabled = true;
                    StatusText.Text = $"Klaar om te wijzigen naar: {_geselecteerdAbonnement.Naam}";
                }
            }
            else if (_geselecteerdAbonnement != null)
            {
                // Geen huidig abonnement, dus kan elk abonnement selecteren
                WijzigButton.Content = $"Kies {_geselecteerdAbonnement.Naam}";
                WijzigButton.Background = System.Windows.Media.Brushes.DodgerBlue;
                WijzigButton.IsEnabled = true;
                StatusText.Text = $"Kies abonnement: {_geselecteerdAbonnement.Naam}";
            }
        }

        private void WijzigAbonnement_Click(object sender, RoutedEventArgs e)
        {
            // 👇 BETER VALIDATIE
            if (_geselecteerdAbonnement == null)
            {
                MessageBox.Show("Selecteer eerst een abonnement uit de lijst!\n\nKlik op een abonnement in de lijst hierboven.", "Selecteer Abonnement");
                return;
            }

            // Debug info
            StatusText.Text = $"Bezig met wijzigen naar: {_geselecteerdAbonnement.Naam}";

            if (_huidigeGebruiker?.AbonnementId == _geselecteerdAbonnement.Id)
            {
                MessageBox.Show("U heeft dit abonnement al!", "Info");
                return;
            }

            try
            {
                string huidigAbonnementNaam = _huidigeGebruiker?.Abonnement?.Naam ?? "Geen abonnement";

                var result = MessageBox.Show(
                    $"Weet u zeker dat u uw abonnement wilt wijzigen?\n\n" +
                    $"Huidig: {huidigAbonnementNaam}\n" +
                    $"Nieuw: {_geselecteerdAbonnement.Naam}\n" +
                    $"Prijs: €{_geselecteerdAbonnement.Prijs:0.00}/maand\n\n" +
                    $"De wijziging gaat direct in.",
                    "Bevestig Abonnement Wijziging",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new FitnessClubDbContext())
                    {
                        // Haal de gebruiker opnieuw op uit de database
                        var gebruiker = context.Users.Find(_huidigeGebruiker?.Id);
                        if (gebruiker != null)
                        {
                            // Wijzig het abonnement
                            gebruiker.AbonnementId = _geselecteerdAbonnement.Id;
                            context.SaveChanges();

                            MessageBox.Show(
                                $"Succesvol gewijzigd naar {_geselecteerdAbonnement.Naam} abonnement!\n\n" +
                                $"Uw nieuwe abonnement:\n" +
                                $"• {_geselecteerdAbonnement.Naam}\n" +
                                $"• €{_geselecteerdAbonnement.Prijs:0.00} per maand\n" +
                                $"• {_geselecteerdAbonnement.Omschrijving}",
                                "Abonnement Gewijzigd",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            // Refresh de data
                            LaadMijnAbonnement();
                            LaadBeschikbareAbonnementen();
                            StatusText.Text = "Succesvol gewijzigd!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij wijzigen abonnement: {ex.Message}", "Fout");
                StatusText.Text = "Fout bij wijzigen";
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LaadMijnAbonnement();
            LaadBeschikbareAbonnementen();
            StatusText.Text = "Vernieuwd";
        }
    }
}