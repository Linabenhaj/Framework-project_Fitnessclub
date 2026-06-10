using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Views
{
    public partial class InschrijvingenOverzicht : UserControl
    {
        private readonly FitnessClubDbContext _context;

        public InschrijvingenOverzicht()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);
            LaadInschrijvingen();
        }

        private void LaadInschrijvingen()
        {
            try
            {
                var inschrijvingen = _context.Inschrijvingen
                    .Include(i => i.Gebruiker)
                    .Include(i => i.Les)
                    .Where(i => i.Status == "Actief")
                    .OrderByDescending(i => i.Les.StartTijd)
                    .ToList();

                InschrijvingenDataGrid.ItemsSource = inschrijvingen;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden inschrijvingen: {ex.Message}");
            }
        }

        // Naam overeenkomstig met XAML Click="VerwijderenClick"
        private void VerwijderenClick(object sender, RoutedEventArgs e)
        {
            if (InschrijvingenDataGrid.SelectedItem is Inschrijving geselecteerdeInschrijving)
            {
                try
                {
                    var inschrijving = _context.Inschrijvingen.Find(geselecteerdeInschrijving.Id);
                    if (inschrijving != null)
                    {
                        _context.Inschrijvingen.Remove(inschrijving);
                        _context.SaveChanges();
                        LaadInschrijvingen();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Fout bij verwijderen inschrijving: {ex.Message}");
                }
            }
        }

        // Naam overeenkomstig met XAML Click="ToevoegenClick"
        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Selecteer een les en een lid om in te schrijven.", "Info");
            LaadInschrijvingen();
        }

        // Naam overeenkomstig met XAML Click="RefreshClick"
        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LaadInschrijvingen();
        }
    }
}