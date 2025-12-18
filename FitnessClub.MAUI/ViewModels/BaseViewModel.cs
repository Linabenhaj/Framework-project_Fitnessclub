using FitnessClub.MAUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private bool isRefreshing;

        public virtual Task OnAppearing() => Task.CompletedTask;
        public virtual Task OnDisappearing() => Task.CompletedTask;
    }
}