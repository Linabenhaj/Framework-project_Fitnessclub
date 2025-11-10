using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FitnessClub.Models.Data;
using System;

namespace FitnessClub.WPF.Views
{
    public partial class MijnInschrijvingen : UserControl
    {
        private string _huidigeGebruikerId;

        public MijnInschrijvingen()
        {
            InitializeComponent();

            
        }

        
        public void SetHuidigeGebruiker(string gebruikerId)
        {
            _huidigeGebruikerId = gebruikerId;
            LaadMijnInschrijvingen();
        }

        private void LaadMijnInschrijvingen()
        {
            try
            {
                if (string.IsNullOrEmpty(_huidigeGebruikerId))
                {
                    GeenInschrijvingenBorder.Visibility = Visibility.Visible;
                    MijnInschrijvingenDataGrid.Visibility = Visibility.Collapsed;
                    return;
                }

                using (var context = new FitnessClubDbContext())
                {
                    var mijnInschrijvingen = context.Inschrijvingen
                        .Include(i => i.Les)
                        .Include(i => i.Gebruiker)
                        .Where(i => i.GebruikerId == _huidigeGebruikerId && !i.IsVerwijderd)
                        .OrderByDescending(i => i.InschrijfDatum)
                        .ToList();

                    MijnInschrijvingenDataGrid.ItemsSource = mijnInschrijvingen;

                    if (!mijnInschrijvingen.Any())
                    {
                        GeenInschrijvingenBorder.Visibility = Visibility.Visible;
                        MijnInschrijvingenDataGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        GeenInschrijvingenBorder.Visibility = Visibility.Collapsed;
                        MijnInschrijvingenDataGrid.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden inschrijvingen: {ex.Message}", "Fout");
                GeenInschrijvingenBorder.Visibility = Visibility.Visible;
                MijnInschrijvingenDataGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void Uitschrijven_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int inschrijvingId)
            {
                try
                {
                    using (var context = new FitnessClubDbContext())
                    {
                        var inschrijving = context.Inschrijvingen
                            .Include(i => i.Les)
                            .FirstOrDefault(i => i.Id == inschrijvingId && i.GebruikerId == _huidigeGebruikerId && !i.IsVerwijderd);

                        if (inschrijving != null)
                        {
                            var result = MessageBox.Show(
                                $"Weet u zeker dat u zich wilt uitschrijven voor:\n\n" +
                                $"• {inschrijving.Les.Naam}\n" +
                                $"• {inschrijving.Les.StartTijd:dd/MM/yyyy HH:mm}",
                                "Bevestig uitschrijving",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                inschrijving.IsVerwijderd = true;
                                inschrijving.VerwijderdOp = DateTime.UtcNow;

                                context.SaveChanges();

                                MessageBox.Show("Succesvol uitgeschreven!", "Uitschrijving Gelukt");
                                LaadMijnInschrijvingen();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Inschrijving niet gevonden.", "Fout");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Fout bij uitschrijven: {ex.Message}", "Fout");
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LaadMijnInschrijvingen();
        }
    }
}