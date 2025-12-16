namespace FitnessClub.MAUI
{
    public static class General
    {
        // API Configuration
        public static string ApiUrl
        {
            get => Preferences.Default.Get(nameof(ApiUrl), "https://localhost:7066/api");
            set => Preferences.Default.Set(nameof(ApiUrl), value);
        }

        public static string BaseApiUrl
        {
            get => Preferences.Default.Get(nameof(BaseApiUrl), "https://localhost:7066");
            set => Preferences.Default.Set(nameof(BaseApiUrl), value);
        }

        // User Info
        public static string UserId
        {
            get => Preferences.Default.Get(nameof(UserId), "");
            set => Preferences.Default.Set(nameof(UserId), value);
        }

        public static string UserEmail
        {
            get => Preferences.Default.Get(nameof(UserEmail), "");
            set => Preferences.Default.Set(nameof(UserEmail), value);
        }

        public static string UserFirstName
        {
            get => Preferences.Default.Get(nameof(UserFirstName), "");
            set => Preferences.Default.Set(nameof(UserFirstName), value);
        }

        public static string UserLastName
        {
            get => Preferences.Default.Get(nameof(UserLastName), "");
            set => Preferences.Default.Set(nameof(UserLastName), value);
        }

        public static string UserRole
        {
            get => Preferences.Default.Get(nameof(UserRole), "");
            set => Preferences.Default.Set(nameof(UserRole), value);
        }

        public static string Token
        {
            get => Preferences.Default.Get(nameof(Token), "");
            set => Preferences.Default.Set(nameof(Token), value);
        }

        // Methods
        // IN General.cs, UPDATE de SaveUserInfo method:
        public static async Task SaveUserInfo(string userId, string email, string firstName, string lastName, string role, string token)
        {
            await Task.Run(() =>
            {
                UserId = userId;
                UserEmail = email;
                UserFirstName = firstName;
                UserLastName = lastName;
                UserRole = role;
                Token = token;

                // Opslaan in Preferences
                Preferences.Default.Set("user_id", userId);
                Preferences.Default.Set("user_email", email);
                Preferences.Default.Set("user_firstname", firstName);
                Preferences.Default.Set("user_lastname", lastName);
                Preferences.Default.Set("user_role", role);
                Preferences.Default.Set("token", token);
            });
        }

        public static async Task LoadUserInfo()
        {
            await Task.CompletedTask; // Simpele async method
        }

        public static void ClearUserInfo()
        {
            Preferences.Default.Remove(nameof(UserId));
            Preferences.Default.Remove(nameof(UserEmail));
            Preferences.Default.Remove(nameof(UserFirstName));
            Preferences.Default.Remove(nameof(UserLastName));
            Preferences.Default.Remove(nameof(UserRole));
            Preferences.Default.Remove(nameof(Token));
        }
    }
}