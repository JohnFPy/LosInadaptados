using Project.domain.models;

namespace Project.infrastucture
{
    public static class UserSession
    {
        public static user? CurrentUser { get; set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void SetCurrentUser(user user)
        {
            CurrentUser = user;
        }

        public static void ClearSession()
        {
            CurrentUser = null;
        }

        public static string GetCurrentUserName()
        {
            return CurrentUser?.Name ?? "Usuario";
        }

        public static string GetCurrentIdUser()
        {
            return CurrentUser?.Id.ToString() ?? "0";
        }

        public static string GetCurrentUsername()
        {
            return CurrentUser?.Username ?? "Unknown";
        }
    }
}