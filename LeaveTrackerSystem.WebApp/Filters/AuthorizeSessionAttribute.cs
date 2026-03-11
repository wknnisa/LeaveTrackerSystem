using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeaveTrackerSystem.WebApp.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        public string? Role { get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;

            var role = session.GetString("Role");

            // Case 1: Not logged in
            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Case 2: Logged in but wrong role
            if (Role != null && role != Role)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
