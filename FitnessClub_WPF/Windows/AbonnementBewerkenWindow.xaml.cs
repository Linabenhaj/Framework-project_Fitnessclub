using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class AbonnementBewerkenWindow : Window
    {
        private readonly FitnessClubDbContext _context = new FitnessClubDbContext();
        private readonly Abonnement _abonnement;

        public AbonnementBewerkenWindow(int abonnementId)
        {
            InitializeComponent();

            // Lambda expression - vind abonnement
            _abonnement = _context.Abonnementen.FirstOrDefault(a => a.Id == abonnementId);

            if (_abonnement != null)
            {
                VulVelden();
            }
        }

        private void VulVelden()
        {
            // Vul de velden met bestaande data
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            // Bewerkingslogica
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}