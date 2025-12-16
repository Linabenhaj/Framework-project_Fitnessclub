using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
                System.Windows.MessageBox.Show($"Fout bij laden inschrijvingen: {ex.Message}");
            }
        }

        private void VerwijderInschrijving_Click(object sender, System.Windows.RoutedEventArgs e)
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
                    System.Windows.MessageBox.Show($"Fout bij verwijderen inschrijving: {ex.Message}");
                }
            }
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LaadInschrijvingen();
        }
    }
}