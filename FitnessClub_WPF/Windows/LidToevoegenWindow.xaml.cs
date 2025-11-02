using FitnessClub.Models;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class LidToevoegenWindow : Window
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext();
        private Lid _teBewerkenLid;

        public LidToevoegenWindow()
        {
            InitializeComponent();
            LaadAbonnementen();
        }

        public LidToevoegenWindow(Lid lid) : this()
        {
            _teBewerkenLid = lid;
            VulVelden();
            Title = "Lid Bewerken";
        }

        private void LaadAbonnementen()
        {
            try
            {
                var abonnementen = _context.Abonnementen
                    .Where(a => !a.IsVerwijderd)
                    .ToList();

                cmbAbonnement.ItemsSource = abonnementen;
                if (abonnementen.Any())
                    cmbAbonnement.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}");
            }
        }

        private void VulVelden()
        {
            if (_teBewerkenLid != null)
            {
                txtVoornaam.Text = _teBewerkenLid.Voornaam;
                txtAchternaam.Text = _teBewerkenLid.Achternaam;
                txtEmail.Text = _teBewerkenLid.Email;
                txtTelefoon.Text = _teBewerkenLid.Telefoon;
                dpGeboortedatum.SelectedDate = _teBewerkenLid.Geboortedatum;

                if (_teBewerkenLid.AbonnementId.HasValue)
                {
                    var abonnement = _context.Abonnementen.Find(_teBewerkenLid.AbonnementId.Value);
                    cmbAbonnement.SelectedItem = abonnement;
                }
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validatie
                if (string.IsNullOrWhiteSpace(txtVoornaam.Text) ||
                    string.IsNullOrWhiteSpace(txtAchternaam.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Vul verplichte velden in!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_teBewerkenLid == null)
                {
                    // Nieuwe lid
                    var nieuwLid = new Lid
                    {
                        Voornaam = txtVoornaam.Text,
                        Achternaam = txtAchternaam.Text,
                        Email = txtEmail.Text,
                        Telefoon = txtTelefoon.Text,
                        Geboortedatum = dpGeboortedatum.SelectedDate ?? DateTime.Now,
                        AbonnementId = (cmbAbonnement.SelectedItem as Abonnement)?.Id,
                        IsVerwijderd = false
                    };

                    _context.Leden.Add(nieuwLid);
                }
                else
                {
                    // Bestaande lid bijwerken
                    _teBewerkenLid.Voornaam = txtVoornaam.Text;
                    _teBewerkenLid.Achternaam = txtAchternaam.Text;
                    _teBewerkenLid.Email = txtEmail.Text;
                    _teBewerkenLid.Telefoon = txtTelefoon.Text;
                    _teBewerkenLid.Geboortedatum = dpGeboortedatum.SelectedDate ?? DateTime.Now;
                    _teBewerkenLid.AbonnementId = (cmbAbonnement.SelectedItem as Abonnement)?.Id;
                }

                _context.SaveChanges();

                MessageBox.Show("Lid succesvol opgeslagen!", "Succes",
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