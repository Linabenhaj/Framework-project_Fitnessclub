using FitnessClub.MAUI.Views;

namespace FitnessClub.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}