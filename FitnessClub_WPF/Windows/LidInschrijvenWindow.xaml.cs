using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Windows
{
    public partial class LidInschrijvenWindow : Window
    {
        private readonly FitnessClubDbContext _context;
        private readonly int _lidId;

        public LidInschrijvenWindow(int lidId)
        {
            InitializeComponent();

            _lidId = lidId;

            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);

            LaadGegevens();
        }

        private void LaadGegevens()
        {
            try
            {
                // Gebruik FindName om controls te vinden
                var txtLidNaam = FindName("txtLidNaam") as TextBlock;
                var dgLessen = FindName("dgLessen") as DataGrid;
                var lessenComboBox = FindName("LessenComboBox") as ComboBox;

                if (txtLidNaam == null || dgLessen == null || lessenComboBox == null)
                {
                    MessageBox.Show("Controls niet gevonden", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Laad lid informatie (gebruik Users)
                var lid = _context.Users
                    .FirstOrDefault(g => g.Id == _lidId.ToString());

                if (lid != null)
                {
                    txtLidNaam.Text = $"{lid.Voornaam} {lid.Achternaam}";
                }

                // Laad beschikbare lessen
                var lessen = _context.Lessen
                    .Where(l => l.IsActief && l.StartTijd > DateTime.Now)
                    .OrderBy(l => l.StartTijd)
                    .ToList();

                dgLessen.ItemsSource = lessen;
                lessenComboBox.ItemsSource = lessen;
                lessenComboBox.DisplayMemberPath = "Naam";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden gegevens: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InschrijvenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dgLessen = FindName("dgLessen") as DataGrid;
                var lessenComboBox = FindName("LessenComboBox") as ComboBox;
                var txtLesInfo = FindName("LesDetailsText") as TextBlock;

                if (dgLessen == null || lessenComboBox == null || txtLesInfo == null)
                {
                    MessageBox.Show("Controls niet gevonden", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (dgLessen.SelectedItem is Les geselecteerdeLes)
                {
                    // Controleer of al ingeschreven
                    var bestaandeInschrijving = _context.Inschrijvingen
                        .FirstOrDefault(i => i.GebruikerId == _lidId.ToString() &&
                                            i.LesId == geselecteerdeLes.Id &&
                                            i.Status == "Actief");

                    if (bestaandeInschrijving != null)
                    {
                        MessageBox.Show("Lid is al ingeschreven voor deze les", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Controleer beschikbare plaatsen
                    var aantalIngeschreven = _context.Inschrijvingen
                        .Count(i => i.LesId == geselecteerdeLes.Id && i.Status == "Actief");

                    if (aantalIngeschreven >= geselecteerdeLes.MaxDeelnemers)
                    {
                        MessageBox.Show("Deze les is vol", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var nieuweInschrijving = new Inschrijving
                    {
                        GebruikerId = _lidId.ToString(),
                        LesId = geselecteerdeLes.Id,
                        InschrijfDatum = DateTime.UtcNow,
                        Status = "Actief"
                    };

                    _context.Inschrijvingen.Add(nieuweInschrijving);
                    _context.SaveChanges();

                    MessageBox.Show("Inschrijving succesvol!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Selecteer eerst een les", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij inschrijven: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void DgLessen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dgLessen = FindName("dgLessen") as DataGrid;
            var txtLesInfo = FindName("LesDetailsText") as TextBlock;

            if (dgLessen == null || txtLesInfo == null) return;

            if (dgLessen.SelectedItem is Les geselecteerdeLes)
            {
                txtLesInfo.Text = $"Les: {geselecteerdeLes.Naam}\n" +
                                 $"Datum: {geselecteerdeLes.StartTijd:dd/MM/yyyy HH:mm}\n" +
                                 $"Trainer: {geselecteerdeLes.Trainer}\n" +
                                 $"Locatie: {geselecteerdeLes.Locatie}";
            }
        }
    }
}