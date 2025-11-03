using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using System;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class AbonnementToevoegenWindow : Window
    {
        public AbonnementToevoegenWindow()
        {
            InitializeComponent();
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validatie van invoer
                if (string.IsNullOrWhiteSpace(NaamTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PrijsTextBox.Text))
                {
                    MessageBox.Show("Naam en prijs zijn verplicht!");
                    return;
                }

                if (!decimal.TryParse(PrijsTextBox.Text, out decimal prijs) ||
                    !int.TryParse(LooptijdTextBox.Text, out int looptijd))
                {
                    MessageBox.Show("Voer geldige getallen in voor prijs en looptijd!");
                    return;
                }

                using (var context = new FitnessClubDbContext())
                {
                    var abonnement = new Abonnement
                    {
                        Naam = NaamTextBox.Text,
                        Prijs = prijs,
                        LooptijdMaanden = looptijd
                    };

                    context.Abonnementen.Add(abonnement);
                    context.SaveChanges();

                    MessageBox.Show("Abonnement toegevoegd!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout: {ex.Message}");
            }
        }

        private void AnnulerenClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}