using System.Windows;

namespace FitnessClub.WPF
{
    public partial class LidToevoegenWindow : Window
    {
        public LidToevoegenWindow()
        {
            InitializeComponent();
        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNaam.Text) || string.IsNullOrWhiteSpace(txtVoornaam.Text))
            {
                txtError.Text = "Naam en voornaam zijn verplicht!";
                return;
            }

            MessageBox.Show($"Lid {txtVoornaam.Text} {txtNaam.Text} succesvol toegevoegd!", "Succes");
            this.DialogResult = true;
            this.Close();
        }
    }
}