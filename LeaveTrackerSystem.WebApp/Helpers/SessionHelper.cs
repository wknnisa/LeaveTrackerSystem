using Microsoft.AspNetCore.Http.Features;

namespace LeaveTrackerSystem.WebApp.Helpers
{
    public static class SessionHelper
    {
        public static string? GetUserEmail(HttpContext context) => context.Session.GetString("Email");
        public static string? GetUserRole(HttpContext context) => context.Session.GetString("Role");
        public static bool IsSessionActive(HttpContext context)
        {
            if (context == null)
            {
                return false;
            }

            var sessionFeature = context.Features.Get<ISessionFeature>();
            if (sessionFeature?.Session == null)
            {
                return false;
            }

            var hasAnyKey = context.Session.Keys.Any();

            if (!hasAnyKey)
            {
                return false;
            }

            var email = GetUserEmail(context);
            var role = GetUserRole(context);

            return !(string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(role));
        }
    }
}
