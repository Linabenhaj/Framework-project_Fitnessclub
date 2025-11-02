using FitnessClub.Models;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class AbonnementToevoegenWindow : Window
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext();
        private Abonnement _teBewerkenAbonnement;

        public AbonnementToevoegenWindow()
        {
            InitializeComponent();
        }

        public AbonnementToevoegenWindow(Abonnement abonnement) : this()
        {
            _teBewerkenAbonnement = abonnement;
            VulVelden();
            Title = "Abonnement Bewerken";
        }

        private void VulVelden()
        {
            if (_teBewerkenAbonnement != null)
            {
                txtNaam.Text = _teBewerkenAbonnement.Naam;
                txtPrijs.Text = _teBewerkenAbonnement.Prijs.ToString();
                txtLooptijd.Text = _teBewerkenAbonnement.LooptijdMaanden.ToString();
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validatie van invoer
                if (string.IsNullOrWhiteSpace(txtNaam.Text) ||
                    !decimal.TryParse(txtPrijs.Text, out decimal prijs) ||
                    !int.TryParse(txtLooptijd.Text, out int looptijd))
                {
                    MessageBox.Show("Vul alle velden correct in!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_teBewerkenAbonnement == null)
                {
                    //  Nieuw abonnement
                    var nieuwAbonnement = new Abonnement
                    {
                        Naam = txtNaam.Text,
                        Prijs = prijs,
                        LooptijdMaanden = looptijd,
                        IsVerwijderd = false
                    };

                    _context.Abonnementen.Add(nieuwAbonnement);
                }
                else
                {
                    // Bestaand abonnement bijwerken
                    _teBewerkenAbonnement.Naam = txtNaam.Text;
                    _teBewerkenAbonnement.Prijs = prijs;
                    _teBewerkenAbonnement.LooptijdMaanden = looptijd;
                }

                _context.SaveChanges();

                MessageBox.Show("Abonnement succesvol opgeslagen!", "Succes",
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