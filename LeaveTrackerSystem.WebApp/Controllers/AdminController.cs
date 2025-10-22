using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.WebApp.Filters;
using LeaveTrackerSystem.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var allRequests = _adminService.GetAllRequests();

            var total = allRequests.Count();
            var approved = allRequests.Count(r => r.Status == LeaveStatus.Approved);
            var pending = allRequests.Count(r => r.Status == LeaveStatus.Pending);
            var rejected = allRequests.Count(r => r.Status == LeaveStatus.Rejected);

            var leaveTypeCounts = allRequests.Where(r => r.LeaveType != null).GroupBy(r => r.LeaveType!.Name).ToDictionary(g => g.Key, g => g.Count());

            ViewBag.LabelsJson = JsonConvert.SerializeObject(leaveTypeCounts.Keys);
            ViewBag.DataJson = JsonConvert.SerializeObject(leaveTypeCounts.Values);

            ViewBag.Total = total;
            ViewBag.Approved = approved;
            ViewBag.Pending = pending;
            ViewBag.Rejected = rejected;
            ViewBag.Name = HttpContext.Session.GetString("Name") ?? "Admin";

            return View();
        }

        public IActionResult AllRequests()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
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
