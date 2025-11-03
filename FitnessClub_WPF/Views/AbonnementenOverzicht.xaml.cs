using FitnessClub.Models.Models;
using FitnessClub.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Views
{
    public partial class AbonnementenOverzicht : UserControl
    {
        public AbonnementenOverzicht()
        {
            InitializeComponent();
            LoadAbonnementen();
        }

        private void LoadAbonnementen()
        {
            try
            {
                // INQ query syntax en soft delete toegevoegd
                using (var context = new FitnessClubDbContext())
                {           
                    var abonnementen = from abonnement in context.Abonnementen
                                       where !abonnement.IsVerwijderd
                                       select abonnement;

                    AbonnementenDataGrid.ItemsSource = abonnementen.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            var window = new Windows.AbonnementToevoegenWindow();
            window.Closed += (s, args) => LoadAbonnementen();
            window.ShowDialog();
        }

        private void BewerkenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int abonnementId)
            {
                var window = new Windows.AbonnementBewerkenWindow(abonnementId);
                window.Closed += (s, args) => LoadAbonnementen();
                window.ShowDialog();
            }
        }

        private void VerwijderClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int abonnementId)
            {
                try
                {
                    // Lambda expression
                    using (var context = new FitnessClubDbContext())
                    {
                        var abonnement = context.Abonnementen.FirstOrDefault(x => x.Id == abonnementId);
                        if (abonnement != null)
                        {
                            // SOFT DELETE
                            abonnement.IsVerwijderd = true;
                            abonnement.VerwijderdOp = DateTime.Now;
                            context.SaveChanges();
                            LoadAbonnementen();
                            MessageBox.Show("Abonnement verwijderd!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout: {ex.Message}");
                }
            }
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LoadAbonnementen();
        }
    }
}