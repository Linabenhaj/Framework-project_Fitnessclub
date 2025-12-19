using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessClub.MAUI.Models;

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
                PropertyNameCaseInsensitive = true
            };

            // Accept header voor JSON
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Helper om base URL te zien
        public string GetBaseUrl() => _httpClient?.BaseAddress?.ToString() ?? "Not set";

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                Debug.WriteLine($"✅ Token set in HttpClient: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                Debug.WriteLine("❌ Token cleared from HttpClient");
            }
        }

        // Test verbinding
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                Debug.WriteLine($"🔗 Testing connection to: {GetBaseUrl()}");
                var response = await _httpClient.GetAsync("health");
                var isSuccess = response.IsSuccessStatusCode;
                Debug.WriteLine($"🔗 Connection test: {(isSuccess ? "✅ SUCCESS" : "❌ FAILED")}");
                return isSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🔗 Connection test ERROR: {ex.Message}");
                return false;
            }
        }

        // LOGIN
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                Debug.WriteLine($"🔐 Attempting login for: {email}");

                var loginModel = new { Email = email, Password = password };
                string jsonString = JsonSerializer.Serialize(loginModel, _jsonOptions);
                HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("gebruikers/login", content);

                Debug.WriteLine($"🔐 Login response status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"🔐 Login response body: {responseBody}");

                    var result = JsonSerializer.Deserialize<LoginResult>(responseBody, _jsonOptions);

                    if (result != null)
                    {
                        result.Success = true;
                        Debug.WriteLine($"🔐 Login success: Token={(string.IsNullOrEmpty(result.Token) ? "MISSING" : "PRESENT")}");
                        return result;
                    }
                }
                else
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"🔐 Login failed: {response.StatusCode} - {errorBody}");
                }

                return new LoginResult
                {
                    Success = false,
                    Message = "Login mislukt - server error"
                };
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"🔐 Network error: {ex.Message}");
                return new LoginResult
                {
                    Success = false,
                    Message = $"Netwerkfout: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🔐 General error: {ex.Message}");
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
                var response = await _httpClient.GetAsync("gebruikers/validate");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // LESSEN
        public async Task<ApiResponse<List<LocalLes>>> GetAllLessenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("lessen");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ApiResponse<List<LocalLes>>>(json, _jsonOptions)
                        ?? new ApiResponse<List<LocalLes>> { Success = false, Message = "No data" };
                }

                return new ApiResponse<List<LocalLes>>
                {
                    Success = false,
                    Message = $"Server error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<LocalLes>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // Andere methods blijven hetzelfde of kunnen later toegevoegd
        public async Task<ApiResponse<List<GebruikerInfo>>> GetAllUsersAsync()
        {
            return new ApiResponse<List<GebruikerInfo>> { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse<LocalLes>> GetLesByIdAsync(int id)
        {
            return new ApiResponse<LocalLes> { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse<LocalLes>> CreateLesAsync(LocalLes les)
        {
            return new ApiResponse<LocalLes> { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse<LocalLes>> UpdateLesAsync(int id, LocalLes les)
        {
            return new ApiResponse<LocalLes> { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse> DeleteLesAsync(int id)
        {
            return new ApiResponse { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse<List<LocalInschrijving>>> GetUserInschrijvingenAsync(string userId)
        {
            return new ApiResponse<List<LocalInschrijving>> { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse<LocalInschrijving>> CreateInschrijvingAsync(InschrijvingDto dto)
        {
            return new ApiResponse<LocalInschrijving> { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse> DeleteInschrijvingAsync(int id)
        {
            return new ApiResponse { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse<GebruikerInfo>> RegisterAsync(RegistratieDto dto)
        {
            return new ApiResponse<GebruikerInfo> { Success = false, Message = "Not implemented" };
        }

        public async Task<ApiResponse> HealthCheckAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("health");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true };
                }
                return new ApiResponse { Success = false, Message = "API is not healthy" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }
    }

    // DTO CLASSES
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

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class InschrijvingDto
    {
        public string GebruikerId { get; set; } = string.Empty;
        public int LesId { get; set; }
        public DateTime InschrijfDatum { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Actief";
    }

    public class RegistratieDto
    {
        public string Email { get; set; } = string.Empty;
        public string Wachtwoord { get; set; } = string.Empty;
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public string? Telefoon { get; set; }
        public DateTime Geboortedatum { get; set; }
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