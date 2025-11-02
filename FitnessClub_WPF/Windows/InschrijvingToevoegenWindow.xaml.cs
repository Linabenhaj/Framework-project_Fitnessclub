using FitnessClub.Models;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class InschrijvingToevoegenWindow : Window
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext();
        private Inschrijving _teBewerkenInschrijving;

        public InschrijvingToevoegenWindow()
        {
            InitializeComponent();
            LaadLedenEnAbonnementen();
            dpStartDatum.SelectedDate = DateTime.Now;
            dpEindDatum.SelectedDate = DateTime.Now.AddMonths(1);
        }

        public InschrijvingToevoegenWindow(Inschrijving inschrijving) : this()
        {
            _teBewerkenInschrijving = inschrijving;
            VulVelden();
            Title = "Inschrijving Bewerken";
        }

        private void LaadLedenEnAbonnementen()
        {
            try
            {
                // Laad leden
                var leden = _context.Leden
                    .Where(l => !l.IsVerwijderd)
                    .ToList();
                cmbLid.ItemsSource = leden;
                if (leden.Any())
                    cmbLid.SelectedIndex = 0;

                //Laad abonnementen
                var abonnementen = _context.Abonnementen
                    .Where(a => !a.IsVerwijderd)
                    .ToList();
                cmbAbonnement.ItemsSource = abonnementen;
                if (abonnementen.Any())
                    cmbAbonnement.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden data: {ex.Message}");
            }
        }

        private void VulVelden()
        {
            if (_teBewerkenInschrijving != null)
            {
                var lid = _context.Leden.FirstOrDefault(l => l.Id == _teBewerkenInschrijving.LidId);
                var abonnement = _context.Abonnementen.FirstOrDefault(a => a.Id == _teBewerkenInschrijving.AbonnementId);

                cmbLid.SelectedItem = lid;
                cmbAbonnement.SelectedItem = abonnement;
                dpStartDatum.SelectedDate = _teBewerkenInschrijving.StartDatum;
                dpEindDatum.SelectedDate = _teBewerkenInschrijving.EindDatum;
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validatie
                if (cmbLid.SelectedItem == null || cmbAbonnement.SelectedItem == null)
                {
                    MessageBox.Show("Selecteer een lid en abonnement!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var geselecteerdLid = (Lid)cmbLid.SelectedItem;
                var geselecteerdAbonnement = (Abonnement)cmbAbonnement.SelectedItem;

                if (_teBewerkenInschrijving == null)
                {
                    //Nieuwe inschrijving
                    var nieuweInschrijving = new Inschrijving
                    {
                        LidId = geselecteerdLid.Id,
                        AbonnementId = geselecteerdAbonnement.Id,
                        StartDatum = dpStartDatum.SelectedDate ?? DateTime.Now,
                        EindDatum = dpEindDatum.SelectedDate ?? DateTime.Now.AddMonths(1),
                        IsVerwijderd = false
                    };

                    _context.Inschrijvingen.Add(nieuweInschrijving);
                }
                else
                {
                    // Bestaande inschrijving bijwerken
                    _teBewerkenInschrijving.LidId = geselecteerdLid.Id;
                    _teBewerkenInschrijving.AbonnementId = geselecteerdAbonnement.Id;
                    _teBewerkenInschrijving.StartDatum = dpStartDatum.SelectedDate ?? DateTime.Now;
                    _teBewerkenInschrijving.EindDatum = dpEindDatum.SelectedDate ?? DateTime.Now.AddMonths(1);
                }

                _context.SaveChanges();

                MessageBox.Show("Inschrijving succesvol opgeslagen!", "Succes",
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