using FitnessClub.Models.Data;  
using FitnessClub.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;



namespace FitnessClub.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;

        public MainViewModel(ApplicationDbContext context)
        {
            _context = context;
            LoadLeden();
            LoadAbonnementen();
        }

        // Properties
        public ObservableCollection<Lid> Leden { get; set; } = new ObservableCollection<Lid>();
        public ObservableCollection<Abonnement> Abonnementen { get; set; } = new ObservableCollection<Abonnement>();
        public ObservableCollection<Inschrijving> Inschrijvingen { get; set; } = new ObservableCollection<Inschrijving>();

        // Geselecteerde items voor CRUD
        private Lid _geselecteerdLid;
        public Lid GeselecteerdLid
        {
            get => _geselecteerdLid;
            set
            {
                _geselecteerdLid = value;
                OnPropertyChanged(nameof(GeselecteerdLid));
            }
        }

        private Abonnement _geselecteerdAbonnement;
        public Abonnement GeselecteerdAbonnement
        {
            get => _geselecteerdAbonnement;
            set
            {
                _geselecteerdAbonnement = value;
                OnPropertyChanged(nameof(GeselecteerdAbonnement));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Data laden methods
        private void LoadLeden()
        {
            Leden.Clear();
            var leden = _context.Leden.Where(l => l.Verwijderd == DateTime.MaxValue).ToList();
            foreach (var lid in leden)
            {
                Leden.Add(lid);

            }
        }



        private void LoadAbonnementen()
        {
            Abonnementen.Clear();
            var abonnementen = _context.Abonnementen.Where(a => a.Verwijderd == DateTime.MaxValue).ToList();
            foreach (var abonnement in abonnementen)
            {
                Abonnementen.Add(abonnement);
            }
        }

        // CRUD Methods
        public void VoegLidToe(Lid nieuwLid)
        {
            _context.Leden.Add(nieuwLid);
            _context.SaveChanges();
            LoadLeden();
        }

        public void UpdateLid(Lid lid)
        {
            _context.Leden.Update(lid);
            _context.SaveChanges();
            LoadLeden();
        }



        public void VerwijderLid()
        {
            if (GeselecteerdLid != null)
            {
                // Soft delete
                GeselecteerdLid.Verwijderd = DateTime.Now;
                _context.Leden.Update(GeselecteerdLid);
                _context.SaveChanges();
                LoadLeden();
            }
        }

       
    }
}