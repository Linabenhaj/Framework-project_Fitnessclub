namespace FitnessClub.MAUI
{
    public static class General  // Centrale klasse voor app-wide settings en user management
    {
        // API URL voor alle HTTP requests - aangepast voor Android emulator (10.0.2.2)
        public static string ApiUrl => "http://10.0.2.2:5000/api/";

        // User Info properties - opgeslagen in Preferences voor persistentie
        public static string UserId
        {
            get => Preferences.Default.Get("user_id", "");  // Haal user ID op
            set => Preferences.Default.Set("user_id", value);  // Sla user ID op
        }

        public static string UserEmail
        {
            get => Preferences.Default.Get("user_email", "");  // Haal email op
            set => Preferences.Default.Set("user_email", value);  // Sla email op
        }

        public static string UserFirstName
        {
            get => Preferences.Default.Get("user_firstname", "");  // Haal voornaam op
            set => Preferences.Default.Set("user_firstname", value);  // Sla voornaam op
        }

        public static string UserLastName
        {
            get => Preferences.Default.Get("user_lastname", "");  // Haal achternaam op
            set => Preferences.Default.Set("user_lastname", value);  // Sla achternaam op
        }

        public static string UserRole
        {
            get => Preferences.Default.Get("user_role", "");  // Haal rol op
            set => Preferences.Default.Set("user_role", value);  // Sla rol op
        }

        public static string Token
        {
            get => Preferences.Default.Get("token", "");  // Haal JWT token op
            set => Preferences.Default.Set("token", value);  // Sla token op
        }

        // Helper properties voor snelle checks
        public static bool IsLoggedIn => !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(UserId);  // Controleer of gebruiker ingelogd is
        public static bool IsAdmin => UserRole?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;  // Controleer admin rol
        public static bool IsTrainer => UserRole?.Equals("Trainer", StringComparison.OrdinalIgnoreCase) == true;  // Controleer trainer rol
        public static bool IsUser => UserRole?.Equals("Gebruiker", StringComparison.OrdinalIgnoreCase) == true;  // Controleer gebruiker rol

        // Sla alle user info in één keer op
        public static void SaveUserInfo(string userId, string email, string firstName, string lastName, string role, string token)
        {
            UserId = userId;
            UserEmail = email;
            UserFirstName = firstName;
            UserLastName = lastName;
            UserRole = role;
            Token = token;
        }

        // Verwijder alle user info (logout)
        public static void ClearUserInfo()
        {
            Preferences.Default.Remove("user_id");
            Preferences.Default.Remove("user_email");
            Preferences.Default.Remove("user_firstname");
            Preferences.Default.Remove("user_lastname");
            Preferences.Default.Remove("user_role");
            Preferences.Default.Remove("token");
        }
    }
}