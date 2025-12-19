using FitnessClub.MAUI.ViewModels;
using System.Diagnostics;
using System.Text;

namespace FitnessClub.MAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // reset login velden
            if (BindingContext is LoginViewModel viewModel)
            {
                viewModel.Email = "";
                viewModel.Password = "";
                viewModel.ErrorMessage = "";
                viewModel.ShowError = false;
            }
        }

        
       
    }
}