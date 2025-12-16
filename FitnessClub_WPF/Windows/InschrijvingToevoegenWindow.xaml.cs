using FitnessClub.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class InschrijvingToevoegenWindow : Window
    {
        private readonly FitnessClubDbContext _context;

        public InschrijvingToevoegenWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);
        }

        private void ToevoegenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Inschrijving toegevoegd!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij toevoegen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}