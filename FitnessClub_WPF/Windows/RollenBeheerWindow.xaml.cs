using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class RollenBeheerWindow : Window
    {
        private FitnessClubDbContext _context;
        private System.Collections.Generic.List<Gebruiker> _alleGebruikers;

        public RollenBeheerWindow()
        {
            InitializeComponent();
            _context = new FitnessClubDbContext();
            LaadGebruikersEnRollen();
        }

        private void LaadGebruikersEnRollen()
        {
            try
            {
                // Voor demo - we tonen gewoon een lege lijst
                _alleGebruikers = new System.Collections.Generic.List<Gebruiker>();

                dgGebruikers.ItemsSource = new[]
                {
                    new { UserName = "admin@fitness.com", Email = "admin@fitness.com", CurrentRole = "Admin" },
                    new { UserName = "lid@example.com", Email = "lid@example.com", CurrentRole = "Lid" }
                };

                // Rollen voor dropdown
                rolColumn.ItemsSource = new[] { "Admin", "Lid" };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private void TxtZoek_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                var zoekterm = txtZoek.Text.ToLower();

                if (string.IsNullOrWhiteSpace(zoekterm) || zoekterm == "zoek gebruiker...")
                {
                    LaadGebruikersEnRollen();
                    return;
                }

                // DEMO: Filter functionaliteit
                var huidigeData = dgGebruikers.ItemsSource as System.Collections.IEnumerable;
                // Voor demo doen we niets met zoeken
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij zoeken: {ex.Message}");
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Rollen bijgewerkt! (Demo functionaliteit)", "Succes",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout",
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