using FitnessClub.Models;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class BetalingToevoegenWindow : Window
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext();
        private Betaling _teBewerkenBetaling;

        public BetalingToevoegenWindow()
        {
            InitializeComponent();
            LaadInschrijvingen();
            dpDatum.SelectedDate = DateTime.Now;
        }

        public BetalingToevoegenWindow(Betaling betaling) : this()
        {
            _teBewerkenBetaling = betaling;
            VulVelden();
            Title = "Betaling Bewerken";
        }

        private void LaadInschrijvingen()
        {
            try
            {
                var inschrijvingen = _context.Inschrijvingen
                    .Where(i => !i.IsVerwijderd)
                    .Include(i => i.Lid)
                    .ToList();

                cmbInschrijving.ItemsSource = inschrijvingen;
                if (inschrijvingen.Any())
                    cmbInschrijving.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden inschrijvingen: {ex.Message}");
            }
        }

        private void VulVelden()
        {
            if (_teBewerkenBetaling != null)
            {
                txtBedrag.Text = _teBewerkenBetaling.Bedrag.ToString();
                dpDatum.SelectedDate = _teBewerkenBetaling.Datum;
                chkIsBetaald.IsChecked = _teBewerkenBetaling.IsBetaald;

                var inschrijving = _context.Inschrijvingen
                    .FirstOrDefault(i => i.Id == _teBewerkenBetaling.InschrijvingId);
                cmbInschrijving.SelectedItem = inschrijving;
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validatie
                if (!decimal.TryParse(txtBedrag.Text, out decimal bedrag) ||
                    cmbInschrijving.SelectedItem == null)
                {
                    MessageBox.Show("Vul alle velden correct in!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var geselecteerdeInschrijving = (Inschrijving)cmbInschrijving.SelectedItem;

                if (_teBewerkenBetaling == null)
                {
                    // Nieuwe betaling
                    var nieuweBetaling = new Betaling
                    {
                        InschrijvingId = geselecteerdeInschrijving.Id,
                        Bedrag = bedrag,
                        Datum = dpDatum.SelectedDate ?? DateTime.Now,
                        IsBetaald = chkIsBetaald.IsChecked ?? false,
                        IsVerwijderd = false
                    };

                    _context.Betalingen.Add(nieuweBetaling);
                }
                else
                {
                    // Bestaande betaling bijwerken
                    _teBewerkenBetaling.InschrijvingId = geselecteerdeInschrijving.Id;
                    _teBewerkenBetaling.Bedrag = bedrag;
                    _teBewerkenBetaling.Datum = dpDatum.SelectedDate ?? DateTime.Now;
                    _teBewerkenBetaling.IsBetaald = chkIsBetaald.IsChecked ?? false;
                }

                _context.SaveChanges();

                MessageBox.Show("Betaling succesvol opgeslagen!", "Succes",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Opslaan mislukt: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}