using System.Windows;
using System.Windows.Controls;

namespace FitnessClub.WPF.Controls
{
    public partial class GebruikerInfoControl : UserControl
    {
        public GebruikerInfoControl()
        {
            InitializeComponent();
        }

        // Data properties 
        public string Naam
        {
            get { return (string)GetValue(NaamProperty); }
            set { SetValue(NaamProperty, value); }
        }

        public static readonly DependencyProperty NaamProperty =
            DependencyProperty.Register("Naam", typeof(string), typeof(GebruikerInfoControl));

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(GebruikerInfoControl));

        public string Rol
        {
            get { return (string)GetValue(RolProperty); }
            set { SetValue(RolProperty, value); }
        }

        public static readonly DependencyProperty RolProperty =
            DependencyProperty.Register("Rol", typeof(string), typeof(GebruikerInfoControl));

        // data in te stellen 
        public void SetGebruikerInfo(string naam, string email, string rol)
        {
            NaamText.Text = naam;
            EmailText.Text = email;
            RolText.Text = rol;
        }
    }
}