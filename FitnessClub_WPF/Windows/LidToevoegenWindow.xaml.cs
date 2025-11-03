using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using System;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class LidToevoegenWindow : Window
    {
        public LidToevoegenWindow()
        {
            InitializeComponent();
            GeboortedatumDatePicker.SelectedDate = DateTime.Now.AddYears(-20);
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(VoornaamTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("Voornaam en email zijn verplicht!");
                    return;
                }

                using (var context = new FitnessClubDbContext())
                {
                    var lid = new Lid
                    {
                        Voornaam = VoornaamTextBox.Text,
                        Achternaam = AchternaamTextBox.Text,
                        Email = EmailTextBox.Text,
                        Telefoon = TelefoonTextBox.Text,
                        Geboortedatum = GeboortedatumDatePicker.SelectedDate ?? DateTime.Now.AddYears(-20)
                    };

                    context.Leden.Add(lid);
                    context.SaveChanges();

                    MessageBox.Show("Lid toegevoegd!");
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