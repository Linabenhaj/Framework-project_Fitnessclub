using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.WPF.Windows
{
    public partial class InschrijvingToevoegenWindow : Window
    {
        public InschrijvingToevoegenWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    // Laad gebruikers
                    var gebruikers = context.Users
                        .Where(u => !u.IsVerwijderd)
                        .ToList();
                    cmbGebruiker.ItemsSource = gebruikers;

                    // Laad lessen
                    var lessen = context.Lessen
                        .Where(l => !l.IsVerwijderd)
                        .ToList();
                    cmbLes.ItemsSource = lessen;

                   
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden data: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validatie
                if (cmbGebruiker.SelectedItem == null)
                {
                    MessageBox.Show("Selecteer een gebruiker!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbLes.SelectedItem == null)
                {
                    MessageBox.Show("Selecteer een les!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

              

                var geselecteerdeGebruiker = (Gebruiker)cmbGebruiker.SelectedItem;
                var geselecteerdeLes = (Les)cmbLes.SelectedItem;

                // Controleer of gebruiker al ingeschreven is voor deze les
                using (var context = new FitnessClubDbContext())
                {
                    var bestaandeInschrijving = context.Inschrijvingen
                        .FirstOrDefault(i => i.GebruikerId == geselecteerdeGebruiker.Id &&
                                           i.LesId == geselecteerdeLes.Id &&
                                           !i.IsVerwijderd);

                    if (bestaandeInschrijving != null)
                    {
                        MessageBox.Show("Deze gebruiker is al ingeschreven voor deze les!", "Fout",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Nieuwe inschrijving aanmaken
                    var nieuweInschrijving = new Inschrijving
                    {
                        GebruikerId = geselecteerdeGebruiker.Id,
                        LesId = geselecteerdeLes.Id,
                        InschrijfDatum = DateTime.Now
                        
                    };

                    context.Inschrijvingen.Add(nieuweInschrijving);
                    context.SaveChanges();
                }

                MessageBox.Show("Inschrijving succesvol toegevoegd!", "Succes",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}