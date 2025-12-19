using System.Diagnostics;
using System.Threading.Tasks;

namespace FitnessClub.MAUI.Services
{
    public class AuthService  // Authenticatie service
    {
        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Login gebruiker via API
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            return await _apiService.LoginAsync(email, password);
        }

        // Registreer nieuwe gebruiker
        public async Task<ApiResponse<GebruikerInfo>> RegisterAsync(RegistratieDto dto)
        {
            return await _apiService.RegisterAsync(dto);
        }

        // Loguit gebruiker
        public void Logout()
        {
            General.ClearUserInfo();  // Wis gebruikersdata
            _apiService.SetToken(null);  // Verwijder token
        }

        // Valideer huidige token
        public async Task<bool> ValidateTokenAsync()
        {
            return await _apiService.ValidateTokenAsync();
        }
    }
}