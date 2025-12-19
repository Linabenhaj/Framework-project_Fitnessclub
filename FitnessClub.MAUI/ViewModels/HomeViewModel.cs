using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "FitnessClub";
        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            //  RegisterPage.xaml gerelateerd
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        [RelayCommand]
        private async Task NavigateAsGuest()
        {
            // Navigeer naar lessen als gast
            await Shell.Current.GoToAsync("//LessenPage");
        }

        [RelayCommand]
        private void ChangeLanguage(string languageCode)
        {
            // Simpele taalwisseling 
            string languageName = GetLanguageName(languageCode);

            // Toon melding
            Application.Current?.MainPage?.DisplayAlert(
                "Taal",
                $"Taal gewijzigd naar: {languageName}",
                "OK"
            );
        }

        private string GetLanguageName(string code)
        {
            return code switch
            {
                "nl" => "Nederlands",
                "en" => "English",
                "fr" => "Français",
                _ => "Onbekend"
            };
        }
    }
}