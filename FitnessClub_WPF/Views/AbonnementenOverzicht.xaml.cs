using FitnessClub.Models.Models;
using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using System.Linq;
using FitnessClub.WPF.Windows;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.WPF.Views
{
    public partial class AbonnementenOverzicht : UserControl
    {
        private static readonly string _connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true";

        public AbonnementenOverzicht()
        {
            InitializeComponent();
            LoadAbonnementen();
        }

        private FitnessClubDbContext MaakContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            return new FitnessClubDbContext(optionsBuilder.Options);
        }

        private void LoadAbonnementen()
        {
            try
            {
                using (var context = MaakContext())
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
            MessageBox.Show("Abonnementen kunnen alleen bewerkt worden.", "Info");
        }

        private void BewerkenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int abonnementId)
            {
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
                        using (var context = MaakContext())
                        {
                            var abonnement = context.Abonnementen.Find(abonnementId);
                            if (abonnement != null)
                            {
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