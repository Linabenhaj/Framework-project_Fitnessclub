using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessClub.MAUI.Services;
using FitnessClub.MAUI.ViewModels;

namespace FitnessClub.MAUI.ViewModels.Admin
{
    public partial class AdminDashboardViewModel : BaseViewModel
    {
        public AdminDashboardViewModel()
        {
            Title = "Admin Dashboard";
        }
    }
}