using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FitnessClub.WPF.Windows;

namespace FitnessClub.WPF.Views
{
    public partial class InschrijvingenOverzicht : UserControl
    {
        private FitnessClubDbContext _context = new FitnessClubDbContext();

        public InschrijvingenOverzicht()
        {
            InitializeComponent();
            LoadInschrijvingen();
        }

        private void LoadInschrijvingen()
        {
            try
            {
                //LINQ query + soft delete en joins
                var actieveInschrijvingen = from inschrijving in _context.Inschrijvingen
                                            where !inschrijving.IsVerwijderd
                                            join lid in _context.Leden on inschrijving.LidId equals lid.Id
                                            join abonnement in _context.Abonnementen on inschrijving.AbonnementId equals abonnement.Id
                                            select new
                                            {
                                                inschrijving.Id,
                                                inschrijving.LidId,
                                                LidNaam = lid.Voornaam + " " + lid.Achternaam,
                                                inschrijving.AbonnementId,
                                                AbonnementNaam = abonnement.Naam,
                                                inschrijving.StartDatum,
                                                inschrijving.EindDatum
                                            };

                dgInschrijvingen.ItemsSource = actieveInschrijvingen.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            var window = new InschrijvingToevoegenWindow();
            if (window.ShowDialog() == true)
            {
                LoadInschrijvingen();
            }
        }

        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
         
            dynamic inschrijving = ((Button)sender).DataContext;
            var inschrijvingEntity = _context.Inschrijvingen.Find(inschrijving.Id);

            var window = new InschrijvingToevoegenWindow(inschrijvingEntity);
            if (window.ShowDialog() == true)
            {
                LoadInschrijvingen();
            }
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic inschrijving = ((Button)sender).DataContext;
                var inschrijvingEntity = _context.Inschrijvingen.Find(inschrijving.Id);

                var result = MessageBox.Show($"Weet je zeker dat je deze inschrijving wilt verwijderen?",
                                           "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // SOFT DELETE
                    inschrijvingEntity.IsVerwijderd = true;
                    _context.SaveChanges();
                    LoadInschrijvingen();

                    MessageBox.Show("Inschrijving succesvol verwijderd!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij verwijderen: {ex.Message}");
            }
        }
    }
}