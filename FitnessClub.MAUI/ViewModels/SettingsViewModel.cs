using FitnessClub.MAUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;

namespace FitnessClub.MAUI.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel  // ViewModel voor instellingen pagina
    {
        private readonly Synchronizer _synchronizer;
        private readonly LocalDbContext _context;

        [ObservableProperty]
        private string lastSyncTime = "Nooit";  // Laatste synchronisatietijd

        [ObservableProperty]
        private bool autoSyncEnabled = true;  // Automatische sync instelling

        [ObservableProperty]
        private bool notifyBeforeLesson = true;  // Notificatie instelling

        [ObservableProperty]
        private string notificationTime = "30";  // Notificatietijd (minuten)

        [ObservableProperty]
        private string appVersion = "1.0.0";  // App versie

        [ObservableProperty]
        private string databaseStatus = "Actief";  // Database status

        [ObservableProperty]
        private long localIdCounter = -1;  // Lokale ID counter

        public SettingsViewModel(Synchronizer synchronizer, LocalDbContext context)
        {
            _synchronizer = synchronizer;
            _context = context;
            Title = "Instellingen";
            LoadSettings();  // Laad instellingen bij opstart
        }

        // Laad app instellingen en versie
        private void LoadSettings()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;  // Haal assembly versie op
                AppVersion = version?.ToString() ?? "1.0.0";
                DatabaseStatus = _synchronizer.DatabaseExists ? "Actief" : "Inactief";  // Controleer database status
            }
            catch
            {
                AppVersion = "1.0.0";
            }
        }

        // Handmatige synchronisatie trigger
        [RelayCommand]
        private async Task ManualSync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                await _synchronizer.SynchronizeAll();  // Voer synchronisatie uit
                LastSyncTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");  // Update tijd
                await Application.Current.MainPage.DisplayAlert("Succes", "Synchronisatie voltooid!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", $"Synchronisatie mislukt: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Sla instellingen op in SecureStorage
        [RelayCommand]
        private async Task SaveSettings()
        {
            try
            {
                await SecureStorage.Default.SetAsync("auto_sync", AutoSyncEnabled.ToString());
                await SecureStorage.Default.SetAsync("notify_before_lesson", NotifyBeforeLesson.ToString());
                await SecureStorage.Default.SetAsync("notification_time", NotificationTime);
                await Application.Current.MainPage.DisplayAlert("Succes", "Instellingen opgeslagen!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", $"Kon instellingen niet opslaan: {ex.Message}", "OK");
            }
        }

        // Maak cache leeg (oude data)
        [RelayCommand]
        private async Task ClearCache()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Cache leegmaken", "Weet je zeker dat je de cache wilt leegmaken?", "Ja", "Nee");

            if (confirm)
            {
                try
                {
                    await _synchronizer.CleanupOldData();  // Verwijder oude lessen
                    await Application.Current.MainPage.DisplayAlert("Succes", "Cache geleegd!", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", $"Kon cache niet leegmaken: {ex.Message}", "OK");
                }
            }
        }

        // Reset database naar initiële staat
        [RelayCommand]
        private async Task ResetDatabase()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Database resetten", "⚠️ WAARSCHUWING: Dit verwijdert ALLE lokale data!\nWeet je zeker?", "Ja, reset", "Annuleren");

            if (confirm)
            {
                try
                {
                    IsBusy = true;
                    await _context.Database.EnsureDeletedAsync();  // Verwijder database
                    await _synchronizer.InitializeDatabase();  // Herinitialiseer met demo data
                    LoadSettings();  // Herlaad instellingen
                    await Application.Current.MainPage.DisplayAlert("Succes", "Database gereset en opnieuw geïnitialiseerd!", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Fout", $"Kon database niet resetten: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}