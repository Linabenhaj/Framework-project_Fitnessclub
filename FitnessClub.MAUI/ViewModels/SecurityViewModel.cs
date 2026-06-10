using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace FitnessClub.MAUI.ViewModels
{
  
    /// SecurityViewModel singleton die de inlog-status bijhoudt
    /// Wordt geïnjecteerd in AppShell en LoginPage.
    
    public class SecurityViewModel : ObservableObject
    {
        private string? _currentEmail;
        public string? CurrentEmail
        {
            get => _currentEmail;
            private set => SetProperty(ref _currentEmail, value);
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            private set => SetProperty(ref _isAdmin, value);
        }

        private string? _fullName;
        public string? FullName
        {
            get => _fullName;
            private set => SetProperty(ref _fullName, value);
        }

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(CurrentEmail);

        public void SetUser(string? email, bool isAdmin, string? fullName = null)
        {
            CurrentEmail = email;
            IsAdmin = isAdmin;
            FullName = fullName;
            OnPropertyChanged(nameof(IsAuthenticated));
            Debug.WriteLine($"[Security] SetUser: Email={email}, IsAdmin={isAdmin}, FullName={fullName}");
        }

        public void Reset()
        {
            CurrentEmail = null;
            IsAdmin = false;
            FullName = null;
            OnPropertyChanged(nameof(IsAuthenticated));
            Debug.WriteLine("[Security] Reset: gebruiker uitgelogd");
        }
    }
}
