using System;
using System.Linq;
using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;

namespace FitnessClub.WPF.Windows
{
    public partial class LidToevoegenWindow : Window
    {
        public LidToevoegenWindow()
        {
            InitializeComponent();
            LoadAbonnementen();
        }

        private void LoadAbonnementen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    // Haal alle abonnementen op
                    var abonnementen = context.Abonnementen.ToList();

                    // Zet ze in de combobox
                    AbonnementComboBox.ItemsSource = abonnementen;
                    AbonnementComboBox.DisplayMemberPath = "Naam";  // Toon de naam
                    AbonnementComboBox.SelectedValuePath = "Id";    // Sla de ID op
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}");
            }
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var nieuwLid = new Lid
                    {
                        Voornaam = VoornaamTextBox.Text,
                        Achternaam = AchternaamTextBox.Text,
                        Email = EmailTextBox.Text,
                        Telefoon = TelefoonTextBox.Text,
                        Geboortedatum = GeboortedatumPicker.SelectedDate ?? DateTime.Now,
                        LidSinds = DateTime.Now,
                        AbonnementId = AbonnementComboBox.SelectedValue as int? // Gebruik SelectedValue
                    };

                    context.Leden.Add(nieuwLid);
                    context.SaveChanges();

                    MessageBox.Show("Lid succesvol toegevoegd!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}");
            }
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}