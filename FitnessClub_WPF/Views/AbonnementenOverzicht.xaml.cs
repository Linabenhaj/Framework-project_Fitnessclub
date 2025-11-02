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
    public partial class AbonnementenOverzicht : UserControl
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext();

        public AbonnementenOverzicht()
        {
            InitializeComponent();
            LoadAbonnementen();
        }

        private void LoadAbonnementen()
        {
            try
            {
                // INQ query syntax en soft delete toegevoegd
                var actieveAbonnementen = from abonnement in _context.Abonnementen
                                          where !abonnement.IsVerwijderd
                                          select abonnement;

                dgAbonnementen.ItemsSource = actieveAbonnementen.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new AbonnementToevoegenWindow();
            if (window.ShowDialog() == true)
            {
                LoadAbonnementen();
            }
        }

        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            var abonnement = (Abonnement)((Button)sender).DataContext;
            var window = new AbonnementToevoegenWindow(abonnement);
            if (window.ShowDialog() == true)
            {
                LoadAbonnementen();
            }
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var abonnement = (Abonnement)((Button)sender).DataContext;
                var result = MessageBox.Show($"Weet je zeker dat je {abonnement.Naam} wilt verwijderen?",
                                           "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // SOFT DELETE
                    abonnement.IsVerwijderd = true;
                    _context.SaveChanges();
                    LoadAbonnementen();

                    MessageBox.Show("Abonnement succesvol verwijderd!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij verwijderen: {ex.Message}");
            }
        }
    }
}