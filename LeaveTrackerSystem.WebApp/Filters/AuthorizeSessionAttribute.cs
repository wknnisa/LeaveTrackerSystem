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

            if (string.IsNullOrEmpty(role) || (Role != null && role != Role))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
