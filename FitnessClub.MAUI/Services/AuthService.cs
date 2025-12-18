using System.Diagnostics;

namespace FitnessClub.MAUI.Services
{
    public class AuthService
    {
        // Hardcoded demo users - later vervangen door API
        private readonly List<UserAccount> _users = new()
        {
            new UserAccount { Email = "admin@fitness.com", Password = "admin123", Role = "Admin", Name = "Beheerder" },
            new UserAccount { Email = "trainer@fitness.com", Password = "trainer123", Role = "Trainer", Name = "Trainer Jan" },
            new UserAccount { Email = "gebruiker@fitness.com", Password = "gebruiker123", Role = "Gebruiker", Name = "John Doe" }
        };

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                await Task.Delay(500); // Simuleer netwerk

                var user = _users.FirstOrDefault(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password);

                if (user != null)
                {
                    // Sla gebruiker info op in General
                    await General.SaveUserInfo(
    Guid.NewGuid().ToString(), // userId
    user.Email,
    user.Name,
    user.Role,
    "demo-jwt-token",           
    "demo-token-123"        
);


                    return new LoginResult
                    {
                        Success = true,
                        User = user,
                        Message = "Login succesvol"
                    };
                }

                return new LoginResult
                {
                    Success = false,
                    Message = "Ongeldige email of wachtwoord"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login error: {ex.Message}");
                return new LoginResult
                {
                    Success = false,
                    Message = "Er is een fout opgetreden bij het inloggen"
                };
            }
        }

        public void Logout()
        {
            General.ClearUserInfo();
        }

        public class UserAccount
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public class LoginResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public UserAccount? User { get; set; }
        }


        // In AuthService.cs
        public class AuthResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
            public UserInfo User { get; set; }
        }

        public class UserInfo
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}