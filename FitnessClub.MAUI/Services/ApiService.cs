using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace FitnessClub.MAUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            _httpClient.BaseAddress = new Uri(General.ApiUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                var loginModel = new { Email = email, Password = password };
                string jsonString = JsonSerializer.Serialize(loginModel, _jsonOptions);
                HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                Uri uri = new Uri(General.ApiUrl + "gebruikers/login");
                HttpResponseMessage response = await _httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResult>(responseBody, _jsonOptions);

                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        SetToken(result.Token);
                        result.Success = true;
                        return result;
                    }
                }

                return new LoginResult
                {
                    Success = false,
                    Message = "Login mislukt"
                };
            }
            catch (HttpRequestException ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = $"Kan geen verbinding maken met server: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = $"Fout: {ex.Message}"
                };
            }
        }

        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                Uri uri = new Uri(General.ApiUrl + "gebruikers/validate");
                HttpResponseMessage response = await _httpClient.GetAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    // DTO classes 
    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }
        public string? Id { get; set; }
        public List<string>? Roles { get; set; }
    }

    public class GebruikerInfo
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }
        public string? Telefoon { get; set; }
        public DateTime Geboortedatum { get; set; }
        public string? Rol { get; set; }
        public AbonnementInfo? Abonnement { get; set; }
        public List<InschrijvingInfo>? Inschrijvingen { get; set; }
    }

    public class AbonnementInfo
    {
        public int Id { get; set; }
        public string? Naam { get; set; }
        public decimal Prijs { get; set; }
    }

    public class InschrijvingInfo
    {
        public int Id { get; set; }
        public string? LesNaam { get; set; }
        public DateTime StartTijd { get; set; }
        public string? Status { get; set; }
    }
}