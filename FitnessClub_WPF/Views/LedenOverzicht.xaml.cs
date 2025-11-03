using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Views
{
    public partial class LedenOverzicht : UserControl
    {
        public LedenOverzicht()
        {
            InitializeComponent();
            LoadLeden();
        }

        private void LoadLeden()
        {
            try
            {
                // LINQ method + soft delete
                using (var context = new FitnessClubDbContext())
                {
                    var leden = from lid in context.Leden
                                where !lid.IsVerwijderd
                                select lid;

                    LedenDataGrid.ItemsSource = leden.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private void ToevoegenClick(object sender, RoutedEventArgs e)
        {
            var window = new Windows.LidToevoegenWindow();
            window.Closed += (s, args) => LoadLeden();
            window.ShowDialog();
        }

        private void BewerkenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int lidId)
            {
                var window = new Windows.LidBewerkenWindow(lidId);
                window.Closed += (s, args) => LoadLeden();
                window.ShowDialog();
            }
        }

        private void VerwijderClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int lidId)
            {
                try
                {
                    // Lambda expression
                    using (var context = new FitnessClubDbContext())
                    {
                        var lid = context.Leden.FirstOrDefault(x => x.Id == lidId);
                        if (lid != null)
                        {
                            // SOFT DELETE
                            lid.IsVerwijderd = true;
                            lid.VerwijderdOp = DateTime.Now;
                            context.SaveChanges();
                            LoadLeden();
                            MessageBox.Show("Lid verwijderd!");
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
            LoadLeden();
        }
    }
}