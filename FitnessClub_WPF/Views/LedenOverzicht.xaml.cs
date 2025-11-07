using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.WPF.Views
{
    public partial class LedenOverzicht : UserControl
    {
        public LedenOverzicht()
        {
            InitializeComponent();
            LoadLeden();
        }

        private void LoadLeden()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var leden = context.Users
                        .Include(u => u.Abonnement)
                        .Where(u => !u.IsVerwijderd)
                        .OrderBy(u => u.Achternaam)
                        .ToList();

                    LedenDataGrid.ItemsSource = leden;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden leden: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            var window = new Windows.LidToevoegenWindow();
            window.Closed += (s, args) => LoadLeden();
            window.ShowDialog();
        }

        private void BewerkenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string gebruikerId)
            {
                using (var context = new FitnessClubDbContext())
                {
                    var gebruiker = context.Users
                        .Include(u => u.Abonnement)
                        .FirstOrDefault(u => u.Id == gebruikerId);

                    if (gebruiker != null)
                    {
                        var window = new Windows.LidBewerkenWindow(gebruiker);
                        window.Closed += (s, args) => LoadLeden();
                        window.ShowDialog();
                    }
                }
            }
        }

        private void VerwijderClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string gebruikerId)
            {
                try
                {
                    var result = MessageBox.Show("Weet u zeker dat u dit lid wilt verwijderen?",
                        "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new FitnessClubDbContext())
                        {
                            var gebruiker = context.Users.Find(gebruikerId);
                            if (gebruiker != null)
                            {
                                context.Users.Remove(gebruiker);
                                context.SaveChanges();
                                LoadLeden();
                                MessageBox.Show("Lid verwijderd!");
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
            LoadLeden();
        }
    }
}