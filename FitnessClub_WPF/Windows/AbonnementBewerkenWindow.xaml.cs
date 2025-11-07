using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class AbonnementBewerkenWindow : Window
    {
        private readonly int _abonnementId;
        private readonly FitnessClubDbContext _context = new FitnessClubDbContext();

        public AbonnementBewerkenWindow(int abonnementId)
        {
            InitializeComponent();
            _abonnementId = abonnementId;
            LaadAbonnement();
        }

        private void LaadAbonnement()
        {
            try
            {
                // Lambda expression vind abonnement
                var abonnement = _context.Abonnementen.FirstOrDefault(a => a.Id == _abonnementId);

                if (abonnement != null)
                {
                    NaamTextBox.Text = abonnement.Naam;
                    PrijsTextBox.Text = abonnement.Prijs.ToString("F2");
                    LooptijdTextBox.Text = abonnement.LooptijdMaanden.ToString();
                    OmschrijvingTextBox.Text = abonnement.Omschrijving;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnement: {ex.Message}");
            }
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
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
                {
                    looptijd = 1;
                }

                var abonnement = _context.Abonnementen.FirstOrDefault(a => a.Id == _abonnementId);
                if (abonnement != null)
                {
                    abonnement.Naam = NaamTextBox.Text;
                    abonnement.Prijs = prijs;
                    abonnement.LooptijdMaanden = looptijd;
                    abonnement.Omschrijving = OmschrijvingTextBox.Text;

                    _context.SaveChanges();
                    MessageBox.Show("Abonnement bijgewerkt!");
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout: {ex.Message}");
            }
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}