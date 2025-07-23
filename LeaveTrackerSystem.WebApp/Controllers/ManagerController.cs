using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}
