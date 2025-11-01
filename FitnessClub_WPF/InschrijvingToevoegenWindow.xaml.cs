using System.Windows;

namespace FitnessClub.WPF
{
    public partial class InschrijvingToevoegenWindow : Window
    {
        public InschrijvingToevoegenWindow()
        {
            InitializeComponent();
        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (cmbLid.SelectedItem == null || cmbActiviteit.SelectedItem == null)
            {
                txtError.Text = "Selecteer een lid en activiteit!";
                return;
            }

            MessageBox.Show("Inschrijving succesvol toegevoegd!", "Succes");
            this.DialogResult = true;
            this.Close();
        }
    }
}