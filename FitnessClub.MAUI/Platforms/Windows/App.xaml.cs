using Microsoft.UI.Xaml;

// WinUI specifieke applicatie voor Windows Desktop

namespace FitnessClub.MAUI.WinUI
{
    // Applicatie klasse voor Windows
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
