using FitnessClub.Models.Models;
using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FitnessClub.WPF.Windows;

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

        // zonder dubbele success message
        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var nieuweLesWindow = new NieuweLesWindow();
                var result = nieuweLesWindow.ShowDialog();

                // Alleen refreshen als er effectief iets is toegevoegd
                if (result == true)
                {
                    LaadLessen();
                    
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij toevoegen les: {ex.Message}", "Fout");
            }
        }

        // VERWIJDEREN METHODE
        private void VerwijderenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Les les)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Weet u zeker dat u de les '{les.Naam}' wilt verwijderen?\n\n" +
                        $"Start: {les.StartTijd:dd/MM/yyyy HH:mm}\n" +
                        $"Eind: {les.EindTijd:dd/MM/yyyy HH:mm}",
                        "Bevestig verwijdering",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new FitnessClubDbContext())
                        {
                            var lesInDb = context.Lessen.Find(les.Id);
                            if (lesInDb != null)
                            {
                                // Soft delete
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

        // REFRESH METHODE
        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LaadLessen();
        }
    }
}