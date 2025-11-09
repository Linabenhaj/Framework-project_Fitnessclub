using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace FitnessClub.WPF.Windows
{
    public partial class LidInschrijvenWindow : Window
    {
        public LidInschrijvenWindow()
        {
            InitializeComponent();
            LaadLeden();
            LaadLessen();

            // Event handlers voor selectie wijzigingen
            LessenComboBox.SelectionChanged += LessenComboBox_SelectionChanged;
        }

        private void LaadLeden()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var leden = context.Users
                        .Where(u => !u.IsVerwijderd)
                        .OrderBy(u => u.Voornaam)
                        .ThenBy(u => u.Achternaam)
                        .ToList();

                    // Voeg display naam toe voor combobox
                    foreach (var lid in leden)
                    {
                        lid.DisplayNaam = $"{lid.Voornaam} {lid.Achternaam} ({lid.Email})";
                    }

                    LedenComboBox.ItemsSource = leden;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden leden: {ex.Message}");
            }
        }

        private void LaadLessen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var lessen = context.Lessen
                        .Where(l => !l.IsVerwijderd && l.StartTijd > DateTime.Now)
                        .OrderBy(l => l.StartTijd)
                        .ToList();

                    // Voeg display info toe voor combobox
                    foreach (var les in lessen)
                    {
                        les.DisplayInfo = $"{les.Naam} - {les.StartTijd:dd/MM/yyyy HH:mm}";
                    }

                    LessenComboBox.ItemsSource = lessen;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden lessen: {ex.Message}");
            }
        }

        private void LessenComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LessenComboBox.SelectedItem is Les geselecteerdeLes)
            {
                LesDetailsText.Text = $"{geselecteerdeLes.Naam}\n" +
                                    $"Beschrijving: {geselecteerdeLes.Beschrijving}\n" +
                                    $"Start: {geselecteerdeLes.StartTijd:dd/MM/yyyy HH:mm}\n" +
                                    $"Eind: {geselecteerdeLes.EindTijd:dd/MM/yyyy HH:mm}\n" +
                                    $"Max deelnemers: {geselecteerdeLes.MaxDeelnemers}";
            }
            else
            {
                LesDetailsText.Text = "Selecteer een les om details te zien";
            }
        }

        private void InschrijvenClick(object sender, RoutedEventArgs e)
        {
            ValidatieText.Text = "";

            if (LedenComboBox.SelectedItem == null)
            {
                ValidatieText.Text = "Selecteer een lid!";
                return;
            }

            if (LessenComboBox.SelectedItem == null)
            {
                ValidatieText.Text = "Selecteer een les!";
                return;
            }

            try
            {
                var gekozenLid = (Gebruiker)LedenComboBox.SelectedItem;
                var gekozenLes = (Les)LessenComboBox.SelectedItem;

                using (var context = new FitnessClubDbContext())
                {
                    // Controleer of lid al is ingeschreven voor deze les
                    var bestaandeInschrijving = context.Inschrijvingen
                        .FirstOrDefault(i => i.GebruikerId == gekozenLid.Id &&
                                           i.LesId == gekozenLes.Id &&
                                           !i.IsVerwijderd);

                    if (bestaandeInschrijving != null)
                    {
                        ValidatieText.Text = $"{gekozenLid.Voornaam} is al ingeschreven voor deze les!";
                        return;
                    }

                    // Controleer of les vol is
                    var aantalIngeschreven = context.Inschrijvingen
                        .Count(i => i.LesId == gekozenLes.Id && !i.IsVerwijderd);

                    if (aantalIngeschreven >= gekozenLes.MaxDeelnemers)
                    {
                        ValidatieText.Text = "Deze les is vol! Kies een andere les.";
                        return;
                    }

                    var inschrijving = new Inschrijving
                    {
                        GebruikerId = gekozenLid.Id,
                        LesId = gekozenLes.Id,
                        InschrijfDatum = DateTime.Now
                    };

                    context.Inschrijvingen.Add(inschrijving);
                    context.SaveChanges();

                    MessageBox.Show($"{gekozenLid.Voornaam} {gekozenLid.Achternaam} succesvol ingeschreven voor '{gekozenLes.Naam}'!",
                                  "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij inschrijven: {ex.Message}", "Fout");
            }
        }

        private void AnnulerenClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}