using System.Diagnostics;
using System.Threading.Tasks;

namespace FitnessClub.MAUI.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            // Gebruik de echte API
            return await _apiService.LoginAsync(email, password);
        }

        public async Task<ApiResponse<GebruikerInfo>> RegisterAsync(RegistratieDto dto)
        {
            // Gebruik de echte API
            return await _apiService.RegisterAsync(dto);
        }

        public void Logout()
        {
            General.ClearUserInfo();
            _apiService.SetToken(null);
        }

        public async Task<bool> ValidateTokenAsync()
        {
            return await _apiService.ValidateTokenAsync();
        }
    }
}