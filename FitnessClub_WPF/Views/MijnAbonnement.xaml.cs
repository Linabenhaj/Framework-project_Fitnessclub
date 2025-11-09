using FitnessClub.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;

namespace FitnessClub.WPF.Views
{
    public partial class MijnAbonnement : UserControl
    {
        public MijnAbonnement()
        {
            InitializeComponent();
            ToonHuidigAbonnement();
        }

        private void ToonHuidigAbonnement()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    // gebruik eerste lid met abonnement
                    var gebruiker = context.Users
                        .Include(u => u.Abonnement)
                        .FirstOrDefault(u => u.AbonnementId != null);

                    if (gebruiker?.Abonnement != null)
                    {
                        var abonnement = gebruiker.Abonnement;

                        AbonnementNaamText.Text = abonnement.Naam;
                        AbonnementPrijsText.Text = $"€{abonnement.Prijs:0.00} per maand";
                        AbonnementOmschrijvingText.Text = abonnement.Omschrijving ?? "Geen omschrijving beschikbaar";
                        AbonnementLooptijdText.Text = $"Looptijd: {abonnement.LooptijdMaanden} maand(en)";
                    }
                    else
                    {
                        AbonnementNaamText.Text = "Geen abonnement";
                        AbonnementPrijsText.Text = "U heeft momenteel geen actief abonnement";
                        AbonnementOmschrijvingText.Text = "Neem contact op met de fitness club om een abonnement te kiezen.";
                        AbonnementLooptijdText.Text = "";
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden abonnement: {ex.Message}", "Fout");
            }
        }
    }
}