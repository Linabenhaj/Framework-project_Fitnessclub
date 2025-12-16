using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Windows
{
    public partial class BewerkLidWindow : Window
    {
        private readonly FitnessClubDbContext _context;

        // Add these fields to match XAML
        private ComboBox cmbRol;
        private ComboBox cmbAbonnement;
        private CheckBox isGeblokkeerdCheckBox;

        public BewerkLidWindow()
        {
            InitializeComponent();

            // CORRECTE DB CONTEXT INIT
            var optionsBuilder = new DbContextOptionsBuilder<FitnessClubDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FitnessClubDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true");

            _context = new FitnessClubDbContext(optionsBuilder.Options);

            // Initialize controls
            cmbRol = FindName("cmbRol") as ComboBox;
            cmbAbonnement = FindName("cmbAbonnement") as ComboBox;
            isGeblokkeerdCheckBox = FindName("isGeblokkeerdCheckBox") as CheckBox;

            LaadGegevens();
        }

        private void LaadGegevens()
        {
            VulRollenComboBox();
            LaadAbonnementen();
        }

        private void VulRollenComboBox()
        {
            if (cmbRol != null)
            {
                cmbRol.Items.Add("Lid");
                cmbRol.Items.Add("PremiumLid");
                cmbRol.Items.Add("Trainer");
                cmbRol.Items.Add("Admin");
            }
        }

        private void LaadAbonnementen()
        {
            try
            {
                if (cmbAbonnement != null)
                {
                    var abonnementen = _context.Abonnementen.ToList();
                    cmbAbonnement.ItemsSource = abonnementen;
                    cmbAbonnement.DisplayMemberPath = "Naam";
                    cmbAbonnement.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnementen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpslaanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Wijzigingen opgeslagen!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event handler voor CheckBox change
        private void IsGeblokkeerdCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (isGeblokkeerdCheckBox != null)
            {
                // Voeg logica toe voor geblokkeerd status
                MessageBox.Show($"Gebruiker is nu {(isGeblokkeerdCheckBox.IsChecked == true ? "geblokkeerd" : "niet geblokkeerd")}");
            }
        }
    }
}