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
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // Gebruik camelCase voor JSON
                PropertyNameCaseInsensitive = true  // Negeer hoofdlettergevoeligheid
            };

            // Stel Accept header in voor JSON requests
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Helper methode om basis URL te tonen
        public string GetBaseUrl() => _httpClient?.BaseAddress?.ToString() ?? "Not set";

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);  // Voeg Bearer token toe aan headers
                Debug.WriteLine($"✅ Token set in HttpClient: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;  // Verwijder token
                Debug.WriteLine("❌ Token cleared from HttpClient");
            }
        }

        // Test API verbinding
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                Debug.WriteLine($"🔗 Testing connection to: {GetBaseUrl()}");
                var response = await _httpClient.GetAsync("health");  // Roep health endpoint aan
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

        // LOGIN methode - authenticeert gebruiker
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                Debug.WriteLine($"🔐 Attempting login for: {email}");

                var loginModel = new { Email = email, Password = password };
                string jsonString = JsonSerializer.Serialize(loginModel, _jsonOptions);  // Serialiseer naar JSON
                HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("gebruikers/login", content);  // Stuur POST request

                Debug.WriteLine($"🔐 Login response status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"🔐 Login response body: {responseBody}");

                    var result = JsonSerializer.Deserialize<LoginResult>(responseBody, _jsonOptions);  // Deserialiseer response

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

        // Valideert JWT token
        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("gebruikers/validate");  // Roep validatie endpoint aan
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Haalt alle lessen op van API
        public async Task<ApiResponse<List<LocalLes>>> GetAllLessenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("lessen");  // GET request voor lessen

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ApiResponse<List<LocalLes>>>(json, _jsonOptions)  // Deserialiseer lessen
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

        // NIET GEÏMPLEMENTEERDE METHODES (stub methods)
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

        // Health check methode
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

    // DATA TRANSFER OBJECT CLASSES
    public class LoginResult  // Resultaat van login poging
    {
        public bool Success { get; set; }  // Geeft aan of login gelukt is
        public string? Message { get; set; }  // Bericht voor gebruiker
        public string? Token { get; set; }  // JWT token voor authenticatie
        public string? Email { get; set; }  // Email van gebruiker
        public string? Voornaam { get; set; }  // Voornaam van gebruiker
        public string? Achternaam { get; set; }  // Achternaam van gebruiker
        public string? Id { get; set; }  // Unieke ID van gebruiker
        public List<string>? Roles { get; set; }  // Rollen van gebruiker
    }

    public class ApiResponse<T>  // Generieke API response wrapper
    {
        public bool Success { get; set; }  // Succes indicator
        public string? Message { get; set; }  // Optioneel bericht
        public T? Data { get; set; }  // Response data van type T
    }

    public class ApiResponse  // Non-generieke API response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class InschrijvingDto  // Data voor nieuwe inschrijving
    {
        public string GebruikerId { get; set; } = string.Empty;  // ID van gebruiker
        public int LesId { get; set; }  // ID van les
        public DateTime InschrijfDatum { get; set; } = DateTime.Now;  // Datum van inschrijving
        public string Status { get; set; } = "Actief";  // Status van inschrijving
    }

    public class RegistratieDto  // Data voor nieuwe registratie
    {
        public string Email { get; set; } = string.Empty;  // Email adres
        public string Wachtwoord { get; set; } = string.Empty;  // Wachtwoord
        public string Voornaam { get; set; } = string.Empty;  // Voornaam
        public string Achternaam { get; set; } = string.Empty;  // Achternaam
        public string? Telefoon { get; set; }  // Optionele telefoon
        public DateTime Geboortedatum { get; set; }  // Geboortedatum
    }

    public class GebruikerInfo  // Gebruikersinformatie
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }
        public string? Telefoon { get; set; }
        public DateTime Geboortedatum { get; set; }
        public string? Rol { get; set; }  // Rol van gebruiker
        public AbonnementInfo? Abonnement { get; set; }  // Abonnementsinfo
        public List<InschrijvingInfo>? Inschrijvingen { get; set; }  // Inschrijvingen lijst
    }

    public class AbonnementInfo  // Abonnementsinformatie
    {
        public int Id { get; set; }
        public string? Naam { get; set; }
        public decimal Prijs { get; set; }
    }

    public class InschrijvingInfo  // Inschrijvingsinformatie
    {
        public int Id { get; set; }
        public string? LesNaam { get; set; }
        public DateTime StartTijd { get; set; }
        public string? Status { get; set; }
    }
}