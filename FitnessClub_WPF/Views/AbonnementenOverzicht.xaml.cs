using FitnessClub.Models.Models;
using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using System.Linq;
using FitnessClub.WPF.Windows;

namespace FitnessClub.WPF.Views
{
    public partial class AbonnementenOverzicht : UserControl
    {
        public AbonnementenOverzicht()
        {
            InitializeComponent();
            LoadAbonnementen();
        }

        private void LoadAbonnementen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var abonnementen = context.Abonnementen
                        .Where(a => !a.IsVerwijderd)
                        .OrderBy(a => a.Prijs)
                        .ToList();

                    AbonnementenDataGrid.ItemsSource = abonnementen;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            // ✅ TOEVOEGEN KNOP GEDISABLED - alleen bewerken mogelijk
            MessageBox.Show("Abonnementen kunnen alleen bewerkt worden. Nieuwe abonnementen moeten via de database toegevoegd worden.", "Info");
        }

        private void BewerkenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int abonnementId)
            {
                // ✅ ECHT BEWERKINGSVENSTER
                var window = new AbonnementBewerkenWindow(abonnementId);
                if (window.ShowDialog() == true)
                {
                    LoadAbonnementen();
                    MessageBox.Show("Abonnement succesvol bijgewerkt!", "Succes");
                }
            }
        }

        private void VerwijderClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int abonnementId)
            {
                try
                {
                    var result = MessageBox.Show("Weet u zeker dat u dit abonnement wilt verwijderen?",
                        "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new FitnessClubDbContext())
                        {
                            var abonnement = context.Abonnementen.Find(abonnementId);
                            if (abonnement != null)
                            {
                                // Soft delete
                                abonnement.IsVerwijderd = true;
                                context.SaveChanges();
                                LoadAbonnementen();
                                MessageBox.Show("Abonnement verwijderd!");
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
            LoadAbonnementen();
        }
    }
}