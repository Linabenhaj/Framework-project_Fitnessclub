using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;

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
                    var abonnementen = context.Abonnementen
                        .Where(a => !a.IsVerwijderd)
                        .ToList();

                    // Zet ze in de combobox
                    AbonnementComboBox.ItemsSource = abonnementen;
                    AbonnementComboBox.DisplayMemberPath = "Naam";
                    AbonnementComboBox.SelectedValuePath = "Id";

                    if (abonnementen.Any())
                        AbonnementComboBox.SelectedIndex = 0;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}");
            }
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validatie
                if (string.IsNullOrWhiteSpace(VoornaamTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AchternaamTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                    string.IsNullOrWhiteSpace(TelefoonTextBox.Text) ||
                    GeboortedatumPicker.SelectedDate == null)
                {
                    MessageBox.Show("Vul alle verplichte velden in!", "Fout",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var context = new FitnessClubDbContext())
                {
                    var nieuwLid = new Gebruiker
                    {
                        Voornaam = VoornaamTextBox.Text,
                        Achternaam = AchternaamTextBox.Text,
                        Email = EmailTextBox.Text,
                        UserName = EmailTextBox.Text, // Gebruik email als username
                        Telefoon = TelefoonTextBox.Text,
                        Geboortedatum = GeboortedatumPicker.SelectedDate.Value,
                        AbonnementId = AbonnementComboBox.SelectedValue as int?
                    };

                    context.Users.Add(nieuwLid);
                    context.SaveChanges();

                    MessageBox.Show("Lid succesvol toegevoegd!");
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}");
            }
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}