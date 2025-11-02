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
    public partial class LedenOverzicht : UserControl
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext(); 

        public LedenOverzicht()
        {
            InitializeComponent();
            LaadLeden();
        }

        private void LaadLeden()
        {
            try
            {
                // LINQ method + soft delete
                var actieveLeden = _context.Leden
                    .Where(l => !l.IsVerwijderd)
                    .Include(l => l.Abonnement)
                    .ToList();

                dgLeden.ItemsSource = actieveLeden;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new LidToevoegenWindow();
            if (window.ShowDialog() == true)
            {
                LaadLeden();
            }
        }

        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            var lid = (Lid)((Button)sender).DataContext;
            var window = new LidToevoegenWindow(lid);
            if (window.ShowDialog() == true)
            {
                LaadLeden();
            }
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var lid = (Lid)((Button)sender).DataContext;
                var result = MessageBox.Show($"Weet je zeker dat je {lid.Voornaam} {lid.Achternaam} wilt verwijderen?",
                                           "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // SOFT DELETE
                    lid.IsVerwijderd = true;
                    _context.SaveChanges();
                    LaadLeden();

                    MessageBox.Show("Lid succesvol verwijderd!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij verwijderen: {ex.Message}");
            }
        }
    }
}