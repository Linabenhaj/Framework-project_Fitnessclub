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
            await Shell.Current.GoToAsync(nameof(Views.LoginPage));
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }

        [RelayCommand]
        private async Task NavigateAsGuest()
        {
            await Shell.Current.GoToAsync(nameof(Views.LessenPage));
        }
    }
}
