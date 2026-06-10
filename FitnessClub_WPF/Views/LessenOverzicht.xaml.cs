using FitnessClub.Models.Models;
using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FitnessClub.WPF.Windows;

namespace FitnessClub.WPF.Views
{
    public partial class LessenOverzicht : UserControl
    {
        private static readonly string _connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true";

        public LessenOverzicht()
        {
            InitializeComponent();
            LaadLessen();
        }

        private FitnessClubDbContext MaakContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            return new FitnessClubDbContext(optionsBuilder.Options);
        }

        private void LaadLessen()
        {
            try
            {
                using (var context = MaakContext())
                {
                    var lessen = context.Lessen
                        .Where(l => !l.IsVerwijderd)
                        .OrderBy(l => l.StartTijd)
                        .ToList();

                    LessenDataGrid.ItemsSource = lessen;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden lessen: {ex.Message}", "Fout");
            }
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var nieuweLesWindow = new NieuweLesWindow();
                var result = nieuweLesWindow.ShowDialog();
                if (result == true)
                    LaadLessen();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij toevoegen les: {ex.Message}", "Fout");
            }
        }

        private void VerwijderenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Les les)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Weet u zeker dat u de les '{les.Naam}' wilt verwijderen?",
                        "Bevestig verwijdering",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = MaakContext())
                        {
                            var lesInDb = context.Lessen.Find(les.Id);
                            if (lesInDb != null)
                            {
                                lesInDb.IsVerwijderd = true;
                                context.SaveChanges();
                                LaadLessen();
                                MessageBox.Show("Les succesvol verwijderd!", "Succes");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Fout bij verwijderen: {ex.Message}", "Fout");
                }
            }
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LaadLessen();
        }
    }
}