using CommunityToolkit.Mvvm.ComponentModel;

namespace FitnessClub.MAUI.ViewModels
{

    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty] private string appVersion = "1.0.0";

        public SettingsViewModel()
        {
            Title = "Instellingen";
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = v?.ToString(3) ?? "1.0.0";
        }
    }
}
