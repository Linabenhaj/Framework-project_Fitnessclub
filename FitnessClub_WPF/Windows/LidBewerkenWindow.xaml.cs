using FitnessClub.Models.Data;
using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace FitnessClub.WPF.Windows
{
    public partial class LidBewerkenWindow : Window
    {
        private int _lidId;

        public LidBewerkenWindow(int lidId)
        {
            InitializeComponent();
            _lidId = lidId;
            LaadLid();
        }

        private void LaadLid()
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    // Lambda expression
                    var lid = context.Leden.FirstOrDefault(x => x.Id == _lidId);
                    if (lid != null)
                    {
                        VoornaamTextBox.Text = lid.Voornaam;
                        AchternaamTextBox.Text = lid.Achternaam;
                        EmailTextBox.Text = lid.Email;
                        TelefoonTextBox.Text = lid.Telefoon;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout: {ex.Message}");
            }
        }

        private void OpslaanClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new FitnessClubDbContext())
                {
                    var lid = context.Leden.FirstOrDefault(x => x.Id == _lidId);
                    if (lid != null)
                    {
                        lid.Voornaam = VoornaamTextBox.Text;
                        lid.Achternaam = AchternaamTextBox.Text;
                        lid.Email = EmailTextBox.Text;
                        lid.Telefoon = TelefoonTextBox.Text;

                        context.SaveChanges();
                        MessageBox.Show("Opgeslagen!");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout: {ex.Message}");
            }
        }

        private void AnnulerenClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}