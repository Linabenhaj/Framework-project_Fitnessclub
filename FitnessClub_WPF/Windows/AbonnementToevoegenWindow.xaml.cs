using FitnessClub.Models.Data;
using FitnessClub.Models;
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

                if (!decimal.TryParse(PrijsTextBox.Text, out decimal prijs))
                {
                    MessageBox.Show("Voer een geldig getal in voor prijs!");
                    return;
                }

                if (!int.TryParse(LooptijdTextBox.Text, out int looptijd))
                {
                    looptijd = 1; 
                }

                using (var context = new FitnessClubDbContext())
                {
                    var abonnement = new Abonnement
                    {
                        Naam = NaamTextBox.Text,
                        Prijs = prijs,
                        LooptijdMaanden = looptijd,
                        Omschrijving = OmschrijvingTextBox.Text
                    };

                    context.Abonnementen.Add(abonnement);
                    context.SaveChanges();

                    MessageBox.Show("Abonnement toegevoegd!");
                    this.DialogResult = true;
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
            this.DialogResult = false;
            this.Close();
        }
    }
}