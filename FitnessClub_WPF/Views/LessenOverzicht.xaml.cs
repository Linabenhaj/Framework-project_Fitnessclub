using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models;
using System.Linq;
using FitnessClub.Models.Data;
namespace FitnessClub.WPF.Views
{
    public partial class LessenOverzicht : UserControl
    {
        public LessenOverzicht()
        {
            InitializeComponent();
            LaadLessen();
        }

        private void LaadLessen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var lessen = context.Lessen
                        .Where(l => !l.IsVerwijderd)
                        .OrderBy(l => l.DatumTijd)
                        .ToList();

                    LessenDataGrid.ItemsSource = lessen;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden lessen: {ex.Message}", "Fout");
            }
        }

        private void Inschrijven_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int lesId)
            {
                try
                {
                    using (var context = new FitnessClubDbContext())
                    {
                        var les = context.Lessen.Find(lesId);
                        if (les != null)
                        {
                            var result = MessageBox.Show($"Weet u zeker dat u zich wilt inschrijven voor:\n\n" +
                                                       $"• {les.Naam}\n" +
                                                       $"• {les.DatumTijd:dd/MM/yyyy HH:mm}\n" +
                                                       $"• {les.Duur} minuten",
                                                       "Bevestig inschrijving",
                                                       MessageBoxButton.YesNo,
                                                       MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                var inschrijving = new Inschrijving
                                {
                                    LesId = lesId,
                                    GebruikerId = context.Users.First().Id,
                                    InschrijfDatum = DateTime.Now
                                };

                                context.Inschrijvingen.Add(inschrijving);
                                context.SaveChanges();

                                MessageBox.Show($"Succesvol ingeschreven voor {les.Naam}!", "Inschrijving Gelukt");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Fout bij inschrijven: {ex.Message}", "Fout");
                }
            }
        }

        private void ToonInschrijvingen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ga naar het tabblad 'Mijn Inschrijvingen' om uw inschrijvingen te bekijken.", "Info");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LaadLessen();
        }
    }
}