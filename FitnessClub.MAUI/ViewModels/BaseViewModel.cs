using FitnessClub.Models.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FitnessClub.MAUI.ViewModels
{
    // basisklasse voor alle ViewModels
    public partial class BaseViewModel : ObservableObject
    {
        // titel van de pagina
        [ObservableProperty]
        private string title = string.Empty;

        // true tijdens API calls om de UI te blokkeren
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        // voor pull-to-refresh in lijsten
        [ObservableProperty]
        private bool isRefreshing;

        public virtual Task OnAppearing() => Task.CompletedTask;
        public virtual Task OnDisappearing() => Task.CompletedTask;
    }
}
