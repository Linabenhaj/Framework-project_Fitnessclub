using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class AbonnementBewerkenWindow : Window
    {
        private readonly int _abonnementId;
        private readonly FitnessClubDbContext _context;

        public AbonnementBewerkenWindow(int abonnementId)
        {
            InitializeComponent();
            _abonnementId = abonnementId;

            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);
            LaadAbonnement();
        }

        private void LaadAbonnement()
        {
            try
            {
                var abonnement = _context.Abonnementen.FirstOrDefault(a => a.Id == _abonnementId);

                if (abonnement != null)
                {
                    NaamTextBox.Text = abonnement.Naam;
                    PrijsTextBox.Text = abonnement.Prijs.ToString("F2");
                    LooptijdTextBox.Text = abonnement.DuurInMaanden.ToString();
                    OmschrijvingTextBox.Text = abonnement.Beschrijving;  // Omschrijving → Beschrijving
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnement: {ex.Message}");
            }
        }

        private void OpslaanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NaamTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PrijsTextBox.Text))
                {
                    MessageBox.Show("Naam en prijs zijn verplicht!");
                    return;
                }

                if (!decimal.TryParse(PrijsTextBox.Text, out decimal prijs))
                {
                    MessageBox.Show("Voer een geldig getal in voor prijs!");
                    return;
                }

                if (!int.TryParse(LooptijdTextBox.Text, out int looptijd))
                    looptijd = 1;

                var abonnement = _context.Abonnementen.FirstOrDefault(a => a.Id == _abonnementId);
                if (abonnement != null)
                {
                    abonnement.Naam = NaamTextBox.Text;
                    abonnement.Prijs = prijs;
                    abonnement.DuurInMaanden = looptijd;
                    abonnement.Beschrijving = OmschrijvingTextBox.Text;  // Omschrijving → Beschrijving

                    _context.SaveChanges();
                    MessageBox.Show("Abonnement bijgewerkt!");
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout: {ex.Message}");
            }
        }

        private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}