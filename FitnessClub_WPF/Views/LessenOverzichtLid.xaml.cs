using System.Windows;
using System.Windows.Controls;
using FitnessClub.Models.Data;
using FitnessClub.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.WPF.Views
{
    public partial class LessenOverzichtLid : UserControl
    {
        public LessenOverzichtLid()
        {
            InitializeComponent();
            LaadBeschikbareLessen();
        }

        private void LaadBeschikbareLessen()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    // Toon ALLE beschikbare lessen (niet verwijderd)
                    var lessen = context.Lessen
                        .Where(l => !l.IsVerwijderd)
                        .OrderBy(l => l.StartTijd)
                        .ToList();

                    LessenDataGrid.ItemsSource = lessen;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden lessen: {ex.Message}", "Fout");
            }
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LaadBeschikbareLessen();
        }
    }
}