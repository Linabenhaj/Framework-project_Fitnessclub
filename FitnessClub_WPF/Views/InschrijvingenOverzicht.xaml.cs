using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.WPF.Views
{
    public partial class InschrijvingenOverzicht : UserControl
    {
        public InschrijvingenOverzicht()
        {
            InitializeComponent();
            LoadInschrijvingen();
        }

        private void LoadInschrijvingen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var inschrijvingen = context.Inschrijvingen
                        .Include(i => i.Gebruiker)
                        .Include(i => i.Les)
                        .Where(i => !i.IsVerwijderd)
                        .OrderByDescending(i => i.InschrijfDatum)
                        .ToList();

                    InschrijvingenDataGrid.ItemsSource = inschrijvingen;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden inschrijvingen: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            var window = new Windows.InschrijvingToevoegenWindow();
            if (window.ShowDialog() == true)
            {
                LoadInschrijvingen();
            }
        }

 

        private void VerwijderenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Models.Inschrijving inschrijving)
            {
                try
                {
                    var result = MessageBox.Show("Weet u zeker dat u deze inschrijving wilt verwijderen?",
                        "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new FitnessClubDbContext())
                        {
                            var inschrijvingInDb = context.Inschrijvingen.Find(inschrijving.Id);
                            if (inschrijvingInDb != null)
                            {
                                // Soft delete
                                inschrijvingInDb.IsVerwijderd = true;
                                context.SaveChanges();
                                LoadInschrijvingen();
                                MessageBox.Show("Inschrijving verwijderd!");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Fout bij verwijderen: {ex.Message}");
                }
            }
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LoadInschrijvingen();
        }
    }
}