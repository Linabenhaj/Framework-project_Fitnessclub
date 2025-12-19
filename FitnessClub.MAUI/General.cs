namespace FitnessClub.MAUI
{
    public static class General
    {

        public static string ApiUrl => "http://10.0.2.2:5000/api/";
        // User Info
        public static string UserId
        {
            get => Preferences.Default.Get("user_id", "");
            set => Preferences.Default.Set("user_id", value);
        }

        public static string UserEmail
        {
            get => Preferences.Default.Get("user_email", "");
            set => Preferences.Default.Set("user_email", value);
        }

        public static string UserFirstName
        {
            get => Preferences.Default.Get("user_firstname", "");
            set => Preferences.Default.Set("user_firstname", value);
        }

        public static string UserLastName
        {
            get => Preferences.Default.Get("user_lastname", "");
            set => Preferences.Default.Set("user_lastname", value);
        }

        public static string UserRole
        {
            get => Preferences.Default.Get("user_role", "");
            set => Preferences.Default.Set("user_role", value);
        }

        public static string Token
        {
            get => Preferences.Default.Get("token", "");
            set => Preferences.Default.Set("token", value);
        }

        // Helper properties
        public static bool IsLoggedIn => !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(UserId);
        public static bool IsAdmin => UserRole?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
        public static bool IsTrainer => UserRole?.Equals("Trainer", StringComparison.OrdinalIgnoreCase) == true;
        public static bool IsUser => UserRole?.Equals("Gebruiker", StringComparison.OrdinalIgnoreCase) == true;

        // Methods
        public static void SaveUserInfo(string userId, string email, string firstName, string lastName, string role, string token)
        {
            UserId = userId;
            UserEmail = email;
            UserFirstName = firstName;
            UserLastName = lastName;
            UserRole = role;
            Token = token;
        }

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