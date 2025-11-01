using FitnessClub.Models.Data;  // AANGEPASTE USING!
using FitnessClub.WPF.ViewModels;
using System.Windows;

namespace FitnessClub.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // DbContext 
            var dbContext = new ApplicationDbContext();

            
            
            // ViewModel met DbContext
            var mainViewModel = new MainViewModel(dbContext);

          
            
            // MainWindow instellen
            var mainWindow = new MainWindow();
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }
    }
}