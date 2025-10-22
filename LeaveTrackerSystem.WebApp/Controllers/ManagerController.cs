using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.WebApp.Filters;
using LeaveTrackerSystem.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    [AuthorizeSession(Role = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ManagerService _managerService;

        public ManagerController(
            ManagerService managerService)
        {
            _managerService = managerService;
        }

        public IActionResult Index()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired " });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;
            var requests = _managerService.GetAllRequestsForManager(email, null);

            var total = requests.Count();
            var approved = requests.Count(r => r.Status == LeaveStatus.Approved);
            var pending = requests.Count(r => r.Status == LeaveStatus.Pending);
            var rejected = requests.Count(r => r.Status == LeaveStatus.Rejected);

            var monthlyData = requests.Where(r => r.Status == LeaveStatus.Approved).GroupBy(r => new DateTime(r.StartDate.Year, r.StartDate.Month, 1))
                .OrderBy(g => g.Key).ToDictionary(g => g.Key.ToString("MMM yyyy"), g => g.Count());

            ViewBag.LabelsJson = JsonConvert.SerializeObject(monthlyData.Keys);
            ViewBag.DataJson = JsonConvert.SerializeObject(monthlyData.Values);

            ViewBag.Total = total;
            ViewBag.Approved = approved;
            ViewBag.Pending = pending;
            ViewBag.Rejected = rejected;
            ViewBag.Name = HttpContext.Session.GetString("Name") ?? HttpContext.Session.GetString("Role");

            return View();
        }

        public IActionResult AllRequests(string? status)
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;

            var requests = _managerService.GetAllRequestsForManager(email, status);

            ViewBag.statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "", Selected = string.IsNullOrEmpty(status) },
                new SelectListItem { Text = "Pending", Value = "Pending", Selected = status == "Pending" },
                new SelectListItem { Text = "Approved", Value = "Approved", Selected = status == "Approved" },
                new SelectListItem { Text = "Rejected", Value = "Rejected", Selected = status == "Rejected" }
            };

            return View(requests);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            _managerService.UpdateLeaveStatus(id, LeaveStatus.Approved);
            TempData["Success"] = "Leave request approved successfully.";
            return RedirectToAction("AllRequests");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            _managerService.UpdateLeaveStatus(id, LeaveStatus.Rejected);
            TempData["Info"] = "Leave request rejected.";
            return RedirectToAction("AllRequests");
        }
    }
}
