using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.WPF.Views
{
    public partial class MijnInschrijvingen : UserControl
    {
        public MijnInschrijvingen()
        {
            InitializeComponent();
            LaadMijnInschrijvingen();
        }

        private void LaadMijnInschrijvingen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var mijnInschrijvingen = context.Inschrijvingen
                        .Include(i => i.Les)
                        .Include(i => i.Gebruiker)
                        .Where(i => !i.IsVerwijderd)
                        .Take(5)
                        .OrderByDescending(i => i.Les.DatumTijd)
                        .ToList();

                    MijnInschrijvingenDataGrid.ItemsSource = mijnInschrijvingen;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden inschrijvingen: {ex.Message}", "Fout");
            }
        }

        private void NieuweInschrijving_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ga naar de 'Lessen' tab om in te schrijven voor nieuwe lessen!", "Info");
        }

        private void Uitschrijven_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int inschrijvingId)
            {
                var result = MessageBox.Show("Weet u zeker dat u zich wilt uitschrijven voor deze les?",
                                           "Bevestig uitschrijving",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Uitgeschreven voor inschrijving {inschrijvingId} (demo)!", "Succes");
                    LaadMijnInschrijvingen();
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LaadMijnInschrijvingen();
        }
    }
}