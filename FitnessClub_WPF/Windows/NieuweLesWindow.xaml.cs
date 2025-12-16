using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Windows
{
    public partial class NieuweLesWindow : Window
    {
        private readonly FitnessClubDbContext _context;

        public NieuweLesWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);

            VulTrainersComboBox();
        }

        private void VulTrainersComboBox()
        {
            try
            {
                // Gebruik FindName om de ComboBox te vinden
                var trainerComboBox = FindName("cmbTrainer") as ComboBox;
                if (trainerComboBox != null)
                {
                    // Gebruik Users in plaats van Gebruikers
                    var trainers = _context.Users
                        .Where(g => g.Rol == "Trainer" || g.Rol == "Admin")
                        .Select(g => g.Voornaam + " " + g.Achternaam)
                        .Distinct()
                        .ToList();

                    trainerComboBox.ItemsSource = trainers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden trainers: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpslaanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValideerInvoer())
                    return;

                // Gebruik FindName om alle controls te vinden
                var txtNaam = FindName("NaamTextBox") as TextBox;
                var txtBeschrijving = FindName("BeschrijvingTextBox") as TextBox;
                var startDatumPicker = FindName("StartDatumPicker") as DatePicker;
                var startUurComboBox = FindName("StartUurComboBox") as ComboBox;
                var eindDatumPicker = FindName("EindDatumPicker") as DatePicker;
                var eindUurComboBox = FindName("EindUurComboBox") as ComboBox;
                var txtMaxDeelnemers = FindName("MaxDeelnemersTextBox") as TextBox;
                var txtLocatie = FindName("txtLocatie") as TextBox;
                var trainerComboBox = FindName("cmbTrainer") as ComboBox;

                if (txtNaam == null || txtBeschrijving == null || startDatumPicker == null ||
                    startUurComboBox == null || eindDatumPicker == null || eindUurComboBox == null ||
                    txtMaxDeelnemers == null || txtLocatie == null || trainerComboBox == null)
                {
                    MessageBox.Show("Niet alle velden zijn gevonden", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Parse tijd uit ComboBox
                var startUurItem = startUurComboBox.SelectedItem as ComboBoxItem;
                var eindUurItem = eindUurComboBox.SelectedItem as ComboBoxItem;

                if (startUurItem == null || eindUurItem == null)
                {
                    MessageBox.Show("Selecteer start- en eindtijd", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var nieuweLes = new Les
                {
                    Naam = txtNaam.Text,
                    Beschrijving = txtBeschrijving.Text,
                    StartTijd = startDatumPicker.SelectedDate.Value.Add(TimeSpan.Parse(startUurItem.Content.ToString())),
                    EindTijd = eindDatumPicker.SelectedDate.Value.Add(TimeSpan.Parse(eindUurItem.Content.ToString())),
                    MaxDeelnemers = int.Parse(txtMaxDeelnemers.Text),
                    Locatie = txtLocatie.Text,
                    Trainer = trainerComboBox.SelectedItem?.ToString(),
                    IsActief = true
                };

                _context.Lessen.Add(nieuweLes);
                _context.SaveChanges();

                MessageBox.Show("Les succesvol aangemaakt!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij aanmaken les: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValideerInvoer()
        {
            // Gebruik FindName om controls te vinden
            var txtNaam = FindName("NaamTextBox") as TextBox;
            var startDatumPicker = FindName("StartDatumPicker") as DatePicker;
            var startUurComboBox = FindName("StartUurComboBox") as ComboBox;
            var eindDatumPicker = FindName("EindDatumPicker") as DatePicker;
            var eindUurComboBox = FindName("EindUurComboBox") as ComboBox;
            var txtMaxDeelnemers = FindName("MaxDeelnemersTextBox") as TextBox;
            var txtLocatie = FindName("txtLocatie") as TextBox;
            var trainerComboBox = FindName("cmbTrainer") as ComboBox;

            if (txtNaam == null || string.IsNullOrWhiteSpace(txtNaam.Text))
            {
                MessageBox.Show("Voer een naam in voor de les", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (startDatumPicker == null || startDatumPicker.SelectedDate == null || startUurComboBox == null || startUurComboBox.SelectedItem == null)
            {
                MessageBox.Show("Selecteer een startdatum en -tijd", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (eindDatumPicker == null || eindDatumPicker.SelectedDate == null || eindUurComboBox == null || eindUurComboBox.SelectedItem == null)
            {
                MessageBox.Show("Selecteer een einddatum en -tijd", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtMaxDeelnemers == null || !int.TryParse(txtMaxDeelnemers.Text, out int maxDeelnemers) || maxDeelnemers <= 0)
            {
                MessageBox.Show("Voer een geldig aantal deelnemers in", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtLocatie == null || string.IsNullOrWhiteSpace(txtLocatie.Text))
            {
                MessageBox.Show("Voer een locatie in", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (trainerComboBox == null || trainerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Selecteer een trainer", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}