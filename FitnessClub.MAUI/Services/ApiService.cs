using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessClub.Models.Models;   // LocalLes, LocalUser, LocalInschrijving

namespace FitnessClub.MAUI.Services
{
    // Service voor alle HTTP-calls naar de API
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

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string GetBaseUrl() => _httpClient?.BaseAddress?.ToString() ?? "Not set";

        public void SetToken(string? token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                Debug.WriteLine($"Token set: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                Debug.WriteLine("Token cleared");
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("health");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Connection test ERROR: {ex.Message}");
                return false;
            }
        }

        // AUTHENTICATIE 

        // CORRECTE route: api/account/login
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                Debug.WriteLine($"Login poging voor: {email}");
                var loginModel = new { Email = email, Password = password };
                var content = new StringContent(
                    JsonSerializer.Serialize(loginModel, _jsonOptions),
                    Encoding.UTF8, "application/json");

                // Correcte route: account/login  (base URL eindigt op api/)
                var response = await _httpClient.PostAsync("account/login", content);

                Debug.WriteLine($"Login status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Login response: {body}");

                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;

                    JsonElement userEl = root.TryGetProperty("user", out var u) ? u : root;

                    var result = new LoginResult
                    {
                        Success = root.TryGetProperty("success", out var s) && s.GetBoolean(),
                        Token = root.TryGetProperty("token", out var t) ? t.GetString() : null,
                        Id = userEl.TryGetProperty("id", out var id) ? id.GetString() : null,
                        Email = userEl.TryGetProperty("email", out var em) ? em.GetString() : null,
                        Voornaam = userEl.TryGetProperty("voornaam", out var vn) ? vn.GetString() :
                                   userEl.TryGetProperty("firstName", out var fn) ? fn.GetString() : null,
                        Achternaam = userEl.TryGetProperty("achternaam", out var an) ? an.GetString() :
                                     userEl.TryGetProperty("lastName", out var ln) ? ln.GetString() : null,
                    };

                    if (userEl.TryGetProperty("roles", out var rolesEl) ||
                        userEl.TryGetProperty("Roles", out rolesEl))
                    {
                        result.Roles = new List<string>();
                        foreach (var role in rolesEl.EnumerateArray())
                            result.Roles.Add(role.GetString() ?? "");
                    }

                    return result;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Login mislukt: {response.StatusCode} - {errorBody}");
                    return new LoginResult { Success = false, Message = "Login mislukt - controleer uw gegevens" };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login fout: {ex.Message}");
                return new LoginResult { Success = false, Message = $"Fout: {ex.Message}" };
            }
        }

        // Registreer nieuwe gebruiker via API
        public async Task<ApiResponse<GebruikerInfo>> RegisterAsync(RegistratieDto dto)
        {
            try
            {
                var registerModel = new
                {
                    Email = dto.Email,
                    Password = dto.Wachtwoord,
                    FirstName = dto.Voornaam,
                    LastName = dto.Achternaam
                };
                var content = new StringContent(
                    JsonSerializer.Serialize(registerModel, _jsonOptions),
                    Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("account/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<GebruikerInfo> { Success = true, Message = "Account aangemaakt" };
                }

                var errorBody = await response.Content.ReadAsStringAsync();
                return new ApiResponse<GebruikerInfo> { Success = false, Message = "Registratie mislukt" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GebruikerInfo> { Success = false, Message = ex.Message };
            }
        }

        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("account/validate");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // LESSEN

        // Haalt alle lessen op van API  (route: api/activities of api/lessen)
        public async Task<ApiResponse<List<LocalLes>>> GetAllLessenAsync()
        {
            try
            {
                // De Web-project API controller heet LessenApiController => api/lessenapi
                // De standalone API controller heet ActivitiesController => api/activities
                // We proberen eerst api/activities, dan api/lessen als fallback
                var response = await _httpClient.GetAsync("activities");

                if (!response.IsSuccessStatusCode)
                    response = await _httpClient.GetAsync("lessen");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Lessen response: {json.Substring(0, Math.Min(200, json.Length))}");

                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    // Haal de data-array op
                    JsonElement dataEl;
                    if (root.TryGetProperty("data", out dataEl) && dataEl.ValueKind == JsonValueKind.Array)
                    {
                        var lessen = new List<LocalLes>();
                        foreach (var item in dataEl.EnumerateArray())
                        {
                            lessen.Add(MapToLocalLes(item));
                        }
                        return new ApiResponse<List<LocalLes>> { Success = true, Data = lessen };
                    }
                    else if (root.ValueKind == JsonValueKind.Array)
                    {
                        var lessen = new List<LocalLes>();
                        foreach (var item in root.EnumerateArray())
                            lessen.Add(MapToLocalLes(item));
                        return new ApiResponse<List<LocalLes>> { Success = true, Data = lessen };
                    }
                }

                return new ApiResponse<List<LocalLes>> { Success = false, Message = $"Server error: {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<LocalLes>> { Success = false, Message = ex.Message };
            }
        }

        private LocalLes MapToLocalLes(JsonElement item)
        {
            return new LocalLes
            {
                Id = item.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                Naam = item.TryGetProperty("naam", out var nm) ? nm.GetString() ?? "" : "",
                Beschrijving = item.TryGetProperty("beschrijving", out var bs) ? bs.GetString() ?? "" : "",
                StartTijd = item.TryGetProperty("startTijd", out var st) ? DateTime.Parse(st.GetString() ?? DateTime.Now.ToString()) : DateTime.Now,
                EindTijd = item.TryGetProperty("eindTijd", out var et) ? DateTime.Parse(et.GetString() ?? DateTime.Now.ToString()) : DateTime.Now.AddHours(1),
                Locatie = item.TryGetProperty("locatie", out var loc) ? loc.GetString() ?? "" : "",
                Trainer = item.TryGetProperty("trainer", out var tr) ? tr.GetString() ?? "" : "",
                MaxDeelnemers = item.TryGetProperty("maxDeelnemers", out var mx) ? mx.GetInt32() : 20,
                IsActief = item.TryGetProperty("isActief", out var ia) ? ia.GetBoolean() : true,
                LastSynced = DateTime.Now
            };
        }

        // Haal inschrijvingen van gebruiker op
        public async Task<ApiResponse<List<LocalInschrijving>>> GetUserInschrijvingenAsync(string userId)
        {
            try
            {
                // Probeer via bookings of via lessen/mijninschrijvingen
                var response = await _httpClient.GetAsync("bookings");

                if (!response.IsSuccessStatusCode)
                    response = await _httpClient.GetAsync("lessen/mijninschrijvingen");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    JsonElement dataEl;
                    var dataSource = root.TryGetProperty("data", out dataEl) ? dataEl : root;

                    var inschrijvingen = new List<LocalInschrijving>();
                    if (dataSource.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in dataSource.EnumerateArray())
                        {
                            inschrijvingen.Add(new LocalInschrijving
                            {
                                Id = item.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                                GebruikerId = userId,
                                LesId = item.TryGetProperty("lesId", out var li) ? li.GetInt32() :
                                        item.TryGetProperty("activityId", out var ai) ? ai.GetInt32() : 0,
                                InschrijfDatum = item.TryGetProperty("inschrijfDatum", out var isd) ?
                                    DateTime.Parse(isd.GetString() ?? DateTime.Now.ToString()) : DateTime.Now,
                                Status = item.TryGetProperty("status", out var st) ? st.GetString() ?? "Actief" : "Actief"
                            });
                        }
                    }
                    return new ApiResponse<List<LocalInschrijving>> { Success = true, Data = inschrijvingen };
                }

                return new ApiResponse<List<LocalInschrijving>> { Success = false, Message = $"Server error: {response.StatusCode}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<LocalInschrijving>> { Success = false, Message = ex.Message };
            }
        }

        // Schrijf gebruiker in voor les
        public async Task<ApiResponse<LocalInschrijving>> CreateInschrijvingAsync(InschrijvingDto dto)
        {
            try
            {
                var requestBody = new { ActivityId = dto.LesId, Notes = "" };
                var json = JsonSerializer.Serialize(requestBody); // API is case-insensitive
                Debug.WriteLine($"[Inschrijven] POST bookings – body: {json}, LesId={dto.LesId}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("bookings", content);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<LocalInschrijving>
                    {
                        Success = true,
                        Data = new LocalInschrijving
                        {
                            GebruikerId = dto.GebruikerId,
                            LesId = dto.LesId,
                            InschrijfDatum = DateTime.Now,
                            Status = "Actief"
                        }
                    };
                }

                // Lees de echte foutmelding van de API
                var errorBody = await response.Content.ReadAsStringAsync();
                var statusCode = (int)response.StatusCode;
                Debug.WriteLine($"[Inschrijven] Fout: HTTP {statusCode} – {errorBody}");

                var hasAuth = _httpClient.DefaultRequestHeaders.Authorization != null;
                Debug.WriteLine($"[Inschrijven] Authorization header aanwezig: {hasAuth}");

                string message = $"Inschrijven mislukt (HTTP {statusCode})";
                try
                {
                    using var doc = JsonDocument.Parse(errorBody);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("message", out var m) && m.ValueKind != JsonValueKind.Null)
                        message = m.GetString() ?? message;
                    else if (root.TryGetProperty("title", out var t) && t.ValueKind != JsonValueKind.Null)
                        message = t.GetString() ?? message;
                }
                catch { }

                return new ApiResponse<LocalInschrijving> { Success = false, Message = message };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LocalInschrijving> { Success = false, Message = $"Verbindingsfout: {ex.Message}" };
            }
        }

        // Haal ALLE inschrijvingen op (admin)
        public async Task<ApiResponse<List<LocalInschrijving>>> GetAllBookingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("bookings/all");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var dataEl = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;

                    var list = new List<LocalInschrijving>();
                    if (dataEl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in dataEl.EnumerateArray())
                        {
                            var ins = new LocalInschrijving
                            {
                                Id = item.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                                LesId = item.TryGetProperty("lesId", out var li) ? li.GetInt32() : 0,
                                GebruikerId = item.TryGetProperty("gebruikerId", out var gi) ? gi.GetString() ?? "" : "",
                                InschrijfDatum = item.TryGetProperty("inschrijfDatum", out var isd)
                                    ? DateTime.Parse(isd.GetString() ?? DateTime.Now.ToString()) : DateTime.Now,
                                Status = item.TryGetProperty("status", out var st) ? st.GetString() ?? "Actief" : "Actief"
                            };

                            // Les-info meepakken als aanwezig
                            if (item.TryGetProperty("les", out var lesEl) && lesEl.ValueKind == JsonValueKind.Object)
                                ins.Les = MapToLocalLes(lesEl);

                            // Gebruikernaam meepakken
                            if (item.TryGetProperty("gebruiker", out var gEl) && gEl.ValueKind == JsonValueKind.Object)
                            {
                                var voornaam = gEl.TryGetProperty("firstName", out var fn) ? fn.GetString() ?? "" : "";
                                var achternaam = gEl.TryGetProperty("lastName", out var ln) ? ln.GetString() ?? "" : "";
                                ins.GebruikerNaam = $"{voornaam} {achternaam}".Trim();
                                if (string.IsNullOrWhiteSpace(ins.GebruikerNaam))
                                    ins.GebruikerNaam = gEl.TryGetProperty("email", out var em) ? em.GetString() ?? "" : "";
                            }

                            list.Add(ins);
                        }
                    }
                    return new ApiResponse<List<LocalInschrijving>> { Success = true, Data = list };
                }
                return new ApiResponse<List<LocalInschrijving>> { Success = false, Message = "Kon inschrijvingen niet ophalen" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<LocalInschrijving>> { Success = false, Message = ex.Message };
            }
        }

        // Bewerk een les (admin/trainer) — PUT /api/activities/{id}
        public async Task<ApiResponse<LocalLes>> UpdateLesAsync(LocalLes les)
        {
            try
            {
                // Stuur alle bekende velden 
                var body = new
                {
                    id = les.Id,
                    naam = les.Naam,
                    beschrijving = les.Beschrijving,
                    startTijd = les.StartTijd,
                    eindTijd = les.EindTijd,
                    locatie = les.Locatie,
                    trainer = les.Trainer,
                    maxDeelnemers = les.MaxDeelnemers,
                    isActief = les.IsActief,
                    type = "",
                    isVerwijderd = false,
                    looptijdMaanden = 12,
                    aangemaaktOp = les.StartTijd
                };
                var content = new StringContent(
                    JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"activities/{les.Id}", content);
                if (response.IsSuccessStatusCode)
                    return new ApiResponse<LocalLes> { Success = true, Data = les };

                var err = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"UpdateLes fout: {response.StatusCode} – {err}");
                return new ApiResponse<LocalLes> { Success = false, Message = "Bijwerken mislukt" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LocalLes> { Success = false, Message = ex.Message };
            }
        }

        // Claim een les als trainer  PUT /api/activities/{id} met alleen trainer-naam
        public async Task<ApiResponse<LocalLes>> ClaimLesAsync(int lesId, string trainerNaam)
        {
            try
            {
                var response = await _httpClient.GetAsync($"activities/{lesId}");
                if (!response.IsSuccessStatusCode)
                    return new ApiResponse<LocalLes> { Success = false, Message = "Les niet gevonden" };

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var dataEl = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;
                var les = MapToLocalLes(dataEl);
                les.Trainer = trainerNaam;

                return await UpdateLesAsync(les);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LocalLes> { Success = false, Message = ex.Message };
            }
        }

        // Verwijder een les (admin)  zet IsActief op false via DELETE /api/activities/{id}
        public async Task<ApiResponse> DeleteLesAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"activities/{id}");
                if (response.IsSuccessStatusCode)
                    return new ApiResponse { Success = true };

                return new ApiResponse { Success = false, Message = "Verwijderen mislukt" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }

        // Schrijf gebruiker uit voor les
        public async Task<ApiResponse> DeleteInschrijvingAsync(int inschrijvingId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"bookings/{inschrijvingId}");

                if (response.IsSuccessStatusCode)
                    return new ApiResponse { Success = true };

                return new ApiResponse { Success = false, Message = "Uitschrijven mislukt" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }

        // Verwijder een gebruiker (admin only)
        public async Task<ApiResponse> DeleteUserAsync(string userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"gebruikers/{userId}");
                if (response.IsSuccessStatusCode)
                    return new ApiResponse { Success = true };

                var errorBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[DeleteUser] Mislukt: {response.StatusCode} - {errorBody}");
                return new ApiResponse { Success = false, Message = errorBody.Length > 0 ? errorBody : "Verwijderen mislukt" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }

        // Haal alle gebruikers op (admin)
        public async Task<ApiResponse<List<GebruikerInfo>>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("gebruikers");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    // API geeft  terug
                    var dataEl = root.TryGetProperty("data", out var d) ? d : root;

                    var list = new List<GebruikerInfo>();
                    if (dataEl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in dataEl.EnumerateArray())
                        {
                            list.Add(new GebruikerInfo
                            {
                                Id = item.TryGetProperty("id", out var id) ? id.GetString() : null,
                                Email = item.TryGetProperty("email", out var em) ? em.GetString() : null,
                                Voornaam = item.TryGetProperty("voornaam", out var vn) ? vn.GetString() : null,
                                Achternaam = item.TryGetProperty("achternaam", out var an) ? an.GetString() : null,
                                Rol = item.TryGetProperty("rol", out var rol) ? rol.GetString() : "Lid"
                            });
                        }
                    }
                    return new ApiResponse<List<GebruikerInfo>> { Success = true, Data = list };
                }

                return new ApiResponse<List<GebruikerInfo>> { Success = false, Message = "Kon gebruikers niet ophalen" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GebruikerInfo>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<LocalLes>> GetLesByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"activities/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    var dataEl = root.TryGetProperty("data", out var d) ? d : root;
                    return new ApiResponse<LocalLes> { Success = true, Data = MapToLocalLes(dataEl) };
                }
                return new ApiResponse<LocalLes> { Success = false, Message = "Les niet gevonden" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LocalLes> { Success = false, Message = ex.Message };
            }
        }

        // Maak een nieuwe les aan (Admin)
        public async Task<ApiResponse<LocalLes>> CreateLesAsync(NewLesDto dto)
        {
            try
            {
                var body = new
                {
                    naam = dto.Naam,
                    beschrijving = dto.Beschrijving,
                    startTijd = dto.StartTijd,
                    eindTijd = dto.EindTijd,
                    locatie = dto.Locatie,
                    trainer = dto.Trainer,
                    maxDeelnemers = dto.MaxDeelnemers,
                    isActief = dto.IsActief
                };
                var content = new StringContent(
                    JsonSerializer.Serialize(body, _jsonOptions),
                    Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("activities", content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var dataEl = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;
                    return new ApiResponse<LocalLes> { Success = true, Data = MapToLocalLes(dataEl) };
                }
                var err = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"CreateLes fout: {response.StatusCode} - {err}");
                return new ApiResponse<LocalLes> { Success = false, Message = "Aanmaken mislukt" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LocalLes> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> HealthCheckAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("health");
                return response.IsSuccessStatusCode
                    ? new ApiResponse { Success = true }
                    : new ApiResponse { Success = false, Message = "API niet beschikbaar" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }
    }

    // DTOs en resultaat-klassen

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

    public class NewLesDto
    {
        public string Naam { get; set; } = string.Empty;
        public string Beschrijving { get; set; } = string.Empty;
        public DateTime StartTijd { get; set; }
        public DateTime EindTijd { get; set; }
        public string Locatie { get; set; } = string.Empty;
        public string Trainer { get; set; } = string.Empty;
        public int MaxDeelnemers { get; set; } = 20;
        public bool IsActief { get; set; } = true;
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

        // Helpers voor XAML — bepaal welke knoppen worden getoond per rij
        public bool IsLid => Rol?.Equals("Lid", StringComparison.OrdinalIgnoreCase) == true;
        public bool IsTrainer => Rol?.Equals("Trainer", StringComparison.OrdinalIgnoreCase) == true;
        public bool IsAdmin => Rol?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
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