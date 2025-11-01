using System.Windows;

namespace FitnessClub.WPF
{
    public partial class AbonnementToevoegenWindow : Window
    {
        public AbonnementToevoegenWindow()
        {
            InitializeComponent();
        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNaam.Text) || string.IsNullOrWhiteSpace(txtPrijs.Text))
            {
                txtError.Text = "Naam en prijs zijn verplicht!";
                return;
            }

            MessageBox.Show($"Abonnement {txtNaam.Text} succesvol toegevoegd!", "Succes");
            this.DialogResult = true;
            this.Close();
        }

        private void btnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}