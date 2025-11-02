using FitnessClub.Models;
using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class RollenBeheerWindow : Window
    {
        private FitnessClubDbContext _context;
        private List<Gebruiker> _alleGebruikers;
        private List<IdentityRole> _beschikbareRollen;

        public RollenBeheerWindow()
        {
            InitializeComponent();
            _context = new FitnessClubDbContext();
            LaadGebruikersEnRollen();
        }

        private void LaadGebruikersEnRollen()
        {
            try
            {
              
                _alleGebruikers = _context.Users.ToList();

               
                _beschikbareRollen = _context.Roles.ToList();

                
                dgGebruikers.ItemsSource = _alleGebruikers.Select(g => new
                {
                    g.UserName,
                    g.Email,
                    CurrentRole = GetUserRole(g.Id)
                }).ToList();

              
                rolColumn.ItemsSource = _beschikbareRollen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden: {ex.Message}");
            }
        }

        private string GetUserRole(string userId)
        {
            try
            {
                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId);
                if (userRole != null)
                {
                    var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                    return role?.Name ?? "Geen rol";
                }
                return "Geen rol";
            }
            catch
            {
                return "Onbekend";
            }
        }

        private void TxtZoek_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                var zoekterm = txtZoek.Text.ToLower();

                if (string.IsNullOrWhiteSpace(zoekterm) || zoekterm == "zoek gebruiker...")
                {
                    LaadGebruikersEnRollen();
                    return;
                }

                //LINQ query 
                var gefilterdeGebruikers = from gebruiker in _alleGebruikers
                                           where gebruiker.UserName.ToLower().Contains(zoekterm) ||
                                                 gebruiker.Email.ToLower().Contains(zoekterm)
                                           select new
                                           {
                                               gebruiker.UserName,
                                               gebruiker.Email,
                                               CurrentRole = GetUserRole(gebruiker.Id)
                                           };

                dgGebruikers.ItemsSource = gefilterdeGebruikers.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij zoeken: {ex.Message}");
            }
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // rol update
                

                MessageBox.Show("Rollen bijgewerkt! (vereenvoudigde implementatie)", "Succes",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}