using LeaveTrackerSystem.Application.Interfaces;
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
        private readonly IManagerService _managerService;
        private readonly INotificationService _notificationService;

        public ManagerController(
            IManagerService managerService,
            INotificationService notificationService)
        {
            _managerService = managerService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;
            var requests = _managerService.GetAllRequestsForManager(email, null);
            
            var approved = requests.Count(r => r.Status == LeaveStatus.Approved);
            var rejected = requests.Count(r => r.Status == LeaveStatus.Rejected);
            var pending = requests.Count(r => r.Status == LeaveStatus.Pending);
            var total = approved + rejected;

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
                new SelectListItem { Text = LangHelper.Get(HttpContext, "All"), Value = "", Selected = string.IsNullOrEmpty(status) },
                new SelectListItem { Text = LangHelper.Get(HttpContext, "Pending"), Value = "Pending", Selected = status == "Pending" },
                new SelectListItem { Text = LangHelper.Get(HttpContext, "Approved"), Value = "Approved", Selected = status == "Approved" },
                new SelectListItem { Text = LangHelper.Get(HttpContext, "Rejected"), Value = "Rejected", Selected = status == "Rejected" }
            };

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var request = _managerService.UpdateLeaveStatus(id, LeaveStatus.Approved);

            if (request != null)
            {
                var managerEmail = SessionHelper.GetUserEmail(HttpContext);

                if (string.IsNullOrEmpty(managerEmail))
                {
                    TempData["Error"] = LangHelper.Get(HttpContext, "SessionExpired");
                    return RedirectToAction("Login", "Account");
                }

                _notificationService.NotifyLeaveApproval(managerEmail, request, true);
                TempData["Success"] = LangHelper.Get(HttpContext, "LeaveApprovedSuccess");
                TempData["Info"] = LangHelper.Get(HttpContext, "NotificationApprovedSimulated");
                
            }
            else
            {
                TempData["Error"] = LangHelper.Get(HttpContext, "RejectFailedProcessed");
            }

            return RedirectToAction("AllRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            var request = _managerService.UpdateLeaveStatus(id, LeaveStatus.Rejected);

            if (request != null)
            {
                var managerEmail = SessionHelper.GetUserEmail(HttpContext);
                
                if (string.IsNullOrEmpty(managerEmail))
                {
                    TempData["Error"] = LangHelper.Get(HttpContext, "SessionExpired");
                    return RedirectToAction("Login", "Account");
                }

                _notificationService.NotifyLeaveApproval(managerEmail, request, false);
                TempData["Success"] = LangHelper.Get(HttpContext, "LeaveRejectedSuccess");
                TempData["Info"] = LangHelper.Get(HttpContext, "NotificationRejectedSimulated");
            }
            else
            {
                TempData["Error"] = LangHelper.Get(HttpContext, "RejectFailedProcessed");
            }

                return RedirectToAction("AllRequests");
        }
    }
}
