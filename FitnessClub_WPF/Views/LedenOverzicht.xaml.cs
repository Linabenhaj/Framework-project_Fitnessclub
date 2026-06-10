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

        // Naam overeenkomstig met XAML Click="BewerkenClick"
        private void BewerkenClick(object sender, RoutedEventArgs e)
        {
            if (LedenDataGrid.SelectedItem is Gebruiker geselecteerdLid)
            {
                var bewerkWindow = new BewerkLidWindow();
                bewerkWindow.ShowDialog();
                LaadLeden();
            }
        }

        // Naam overeenkomstig met XAML Click="VerwijderClick"
        private void VerwijderClick(object sender, RoutedEventArgs e)
        {
            if (LedenDataGrid.SelectedItem is Gebruiker geselecteerdLid)
            {
                try
                {
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

        // Naam overeenkomstig met XAML Click="RefreshClick"
        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LaadLeden();
        }
    }
}