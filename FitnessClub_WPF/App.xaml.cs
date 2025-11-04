using System;
using System.Windows;
using FitnessClub.Models.Data;

namespace FitnessClub.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Simpele database setup
                using (var context = new FitnessClubDbContext())
                {
                    // Zorg dat database bestaat
                    context.Database.EnsureCreated();
                }

                // Start direct met main window
                new MainWindow().Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Opstartfout: {ex.Message}");
                // Toch proberen te starten
                new MainWindow().Show();
            }
        }
    }
}