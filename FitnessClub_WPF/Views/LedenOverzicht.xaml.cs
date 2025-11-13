using FitnessClub.Models;
using FitnessClub.Models.Data;
using FitnessClub.WPF.Windows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        //  Bewerken button click (nieuw)
        private void BewerkenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Gebruiker gebruiker)
            {
                try
                {
                    // Open BewerkLidWindow met de gebruiker ID
                    var bewerkWindow = new BewerkLidWindow(gebruiker.Id);
                    var result = bewerkWindow.ShowDialog(); //wacht ops luiting

                    // Refresh de lijst als er wijzigingen zijn opgeslagen
                    if (result == true) 
                    {
                        LoadLeden();
                        MessageBox.Show("Gebruiker succesvol bijgewerkt!", "Succes");
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Fout bij openen bewerkvenster: {ex.Message}", "Fout");
                }
            }
        }

        private void VerwijderClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Gebruiker gebruiker)
            {
                try
                {
                    var result = MessageBox.Show($"Weet u zeker dat u {gebruiker.Voornaam} {gebruiker.Achternaam} wilt verwijderen?",
                        "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (var context = new FitnessClubDbContext())
                        {
                            var gebruikerInDb = context.Users.Find(gebruiker.Id);
                            if (gebruikerInDb != null)
                            {
                                context.Users.Remove(gebruikerInDb);
                                context.SaveChanges();
                                LoadLeden();
                                MessageBox.Show("Lid succesvol verwijderd!", "Succes");
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
            LoadLeden();
        }
    }
}