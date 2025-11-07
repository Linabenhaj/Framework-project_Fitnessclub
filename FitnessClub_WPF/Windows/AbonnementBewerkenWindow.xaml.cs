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

< Window x: Class = "FitnessClub.WPF.Windows.AbonnementBewerkenWindow"
        xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns: x = "http://schemas.microsoft.com/winfx/2006/xaml"
        Title = "Abonnement Bewerken" Height = "350" Width = "400"
        WindowStartupLocation = "CenterOwner" >

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