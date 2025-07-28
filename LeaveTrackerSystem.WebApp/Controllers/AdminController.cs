using LeaveTrackerSystem.Infrastructure.Mock;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            { 
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult AllRequests()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var allRequests = InMemoryData.LeaveRequests;

            return View(allRequests);
        }
    }
}
