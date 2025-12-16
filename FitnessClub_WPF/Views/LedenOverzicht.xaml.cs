using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using FitnessClub.WPF.Windows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Views
{
    public partial class LedenOverzicht : UserControl
    {
        private readonly FitnessClubDbContext _context;

        public LedenOverzicht()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);

            LaadLeden();
        }

        private void LaadLeden()
        {
            try
            {
                // Gebruik Users in plaats van Gebruikers
                var leden = _context.Users
                    .Include(u => u.Abonnement)
                    .Where(u => u.Rol == "Lid" || u.Rol == "PremiumLid")
                    .ToList();

                LedenDataGrid.ItemsSource = leden;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden leden: {ex.Message}");
            }
        }

        private void BewerkLid_Click(object sender, RoutedEventArgs e)
        {
            if (LedenDataGrid.SelectedItem is Gebruiker geselecteerdLid)
            {
                // Roep parameterloze constructor aan
                var bewerkWindow = new BewerkLidWindow();
                // Je kunt eventueel de gebruiker doorgeven via een property
                bewerkWindow.ShowDialog();
                LaadLeden();
            }
        }

        private void VerwijderLid_Click(object sender, RoutedEventArgs e)
        {
            if (LedenDataGrid.SelectedItem is Gebruiker geselecteerdLid)
            {
                try
                {
                    // Gebruik Users in plaats van Gebruikers
                    var lid = _context.Users.Find(geselecteerdLid.Id);
                    if (lid != null)
                    {
                        _context.Users.Remove(lid);
                        _context.SaveChanges();
                        LaadLeden();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Fout bij verwijderen lid: {ex.Message}");
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LaadLeden();
        }
    }
}