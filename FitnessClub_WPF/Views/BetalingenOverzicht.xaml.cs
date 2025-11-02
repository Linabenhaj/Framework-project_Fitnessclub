using FitnessClub.Models;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FitnessClub.WPF.Windows;

namespace FitnessClub.WPF.Views
{
    public partial class BetalingenOverzicht : UserControl
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext();

        public BetalingenOverzicht()
        {


            InitializeComponent();
            LoadBetalingen();
        }

        private void LoadBetalingen()
        {
            try
            {
                // LINQ method syntax en soft delete
                var actieveBetalingen = _context.Betalingen
                    .Where(b => !b.IsVerwijderd)
                    .Include(b => b.Inschrijving)
                    .ToList();

                dgBetalingen.ItemsSource = actieveBetalingen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new BetalingToevoegenWindow();
            if (window.ShowDialog() == true)
            {
                LoadBetalingen();
            }
        }

        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            var betaling = (Betaling)((Button)sender).DataContext;
            var window = new BetalingToevoegenWindow(betaling);
            if (window.ShowDialog() == true)
            {
                LoadBetalingen();
            }
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var betaling = (Betaling)((Button)sender).DataContext;
                var result = MessageBox.Show($"Weet je zeker dat je betaling van {betaling.Bedrag:C} wilt verwijderen?",
                                           "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // SOFT DELETE


                    betaling.IsVerwijderd = true;
                    _context.SaveChanges();
                    LoadBetalingen();

                    MessageBox.Show("Betaling succesvol verwijderd!");
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij verwijderen: {ex.Message}");
            }
        }
    }
}