using System.Windows;

namespace FitnessClub.WPF
{
    public partial class BetalingToevoegenWindow : Window
    {
        public BetalingToevoegenWindow()
        {
            InitializeComponent();
        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (cmbLid.SelectedItem == null || string.IsNullOrWhiteSpace(txtBedrag.Text))
            {
                txtError.Text = "Selecteer een lid en voer een bedrag in!";
                return;
            }

            MessageBox.Show("Betaling succesvol toegevoegd!", "Succes");
            this.DialogResult = true;
            this.Close();
        }

    }
}