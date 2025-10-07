using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.WebApp.Filters;
using LeaveTrackerSystem.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    [AuthorizeSession(Role = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        public AdminController(
            AdminService adminService) 
        { 
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AllRequests()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account");
            }

            var role = SessionHelper.GetUserRole(HttpContext);

            if (role != "Admin")
            {
                TempData["Error"] = "Unauthorized access.";
                return RedirectToAction("Login", "Account");
            }

            var allRequests = _adminService.GetAllRequests();

            return View(allRequests);
        }
    }
}
