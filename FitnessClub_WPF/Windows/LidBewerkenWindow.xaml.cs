using System.Windows;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.WPF.Windows
{
    public partial class LidBewerkenWindow : Window
    {
        private readonly Gebruiker _teBewerkenLid;

        public LidBewerkenWindow(Gebruiker lid)
        {
            InitializeComponent();
            _teBewerkenLid = lid;
            LoadLidData();
            LoadAbonnementen();
        }

        private void LoadLidData()
        {
            try
            {
                // Vul de velden met de bestaande data van het lid
                VoornaamTextBox.Text = _teBewerkenLid.Voornaam;
                AchternaamTextBox.Text = _teBewerkenLid.Achternaam;
                EmailTextBox.Text = _teBewerkenLid.Email;
                TelefoonTextBox.Text = _teBewerkenLid.Telefoon;
                GeboortedatumPicker.SelectedDate = _teBewerkenLid.Geboortedatum;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden lid data: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAbonnementen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    // Laad alle abonnementen
                    var abonnementen = context.Abonnementen
                        .Where(a => !a.IsVerwijderd)
                        .ToList();

                    AbonnementComboBox.ItemsSource = abonnementen;
                    AbonnementComboBox.DisplayMemberPath = "Naam";
                    AbonnementComboBox.SelectedValuePath = "Id";

                    // Selecteer het huidige abonnement van het lid
                    if (_teBewerkenLid.AbonnementId.HasValue)
                    {
                        var huidigAbonnement = abonnementen.FirstOrDefault(a => a.Id == _teBewerkenLid.AbonnementId.Value);
                        if (huidigAbonnement != null)
                        {
                            AbonnementComboBox.SelectedValue = huidigAbonnement.Id;
                        }
                    }
                    else if (abonnementen.Any())
                    {
                        AbonnementComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpslaanClick(object sender, RoutedEventArgs e)
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

                // Update het lid in de database
                using (var context = new FitnessClubDbContext())
                {
                    var lidInDatabase = context.Users
                        .FirstOrDefault(u => u.Id == _teBewerkenLid.Id);

                    if (lidInDatabase != null)
                    {
                        // Update alle properties
                        lidInDatabase.Voornaam = VoornaamTextBox.Text;
                        lidInDatabase.Achternaam = AchternaamTextBox.Text;
                        lidInDatabase.Email = EmailTextBox.Text;
                        lidInDatabase.UserName = EmailTextBox.Text; // Update ook username
                        lidInDatabase.Telefoon = TelefoonTextBox.Text;
                        lidInDatabase.Geboortedatum = GeboortedatumPicker.SelectedDate.Value;

                        if (AbonnementComboBox.SelectedValue is int abonnementId)
                        {
                            lidInDatabase.AbonnementId = abonnementId;
                        }

                        context.SaveChanges();

                        MessageBox.Show($"Lid {lidInDatabase.Voornaam} {lidInDatabase.Achternaam} succesvol bijgewerkt!", "Succes",
                                      MessageBoxButton.OK, MessageBoxImage.Information);

                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Lid niet gevonden in database!", "Fout",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan wijzigingen: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnnulerenClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}