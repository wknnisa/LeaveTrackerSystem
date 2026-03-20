using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.WebApp.Filters;
using LeaveTrackerSystem.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    [AuthorizeSession(Role = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(
            IAdminService adminService) 
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

            var approved = allRequests.Count(r => r.Status == LeaveStatus.Approved);
            var pending = allRequests.Count(r => r.Status == LeaveStatus.Pending);
            var rejected = allRequests.Count(r => r.Status == LeaveStatus.Rejected);
            var total = allRequests.Count();

            var leaveTypeCounts = allRequests.Where(r => r.LeaveType != null).GroupBy(r => r.LeaveType!.Name).ToDictionary(g => g.Key, g => g.Count());

            ViewBag.LabelsJson = JsonConvert.SerializeObject(leaveTypeCounts.Keys);
            ViewBag.DataJson = JsonConvert.SerializeObject(leaveTypeCounts.Values);

            var hasPieData = leaveTypeCounts.Values.Any(v => v > 0);
            ViewBag.HasPieData = hasPieData;

            ViewBag.Total = total;
            ViewBag.Approved = approved;
            ViewBag.Pending = pending;
            ViewBag.Rejected = rejected;
            ViewBag.Name = HttpContext.Session.GetString("Name") ?? "Admin";

            return View();
        }

        public IActionResult AllRequests(string? status, int page = 1)
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            int pageSize = 10;

            var result = _adminService.GetAllRequests(status, page, pageSize);

            var requests = result.Requests;

            ViewBag.Page = page;
            ViewBag.Status = status;
            ViewBag.HasNextPage = result.HasNextPage;

            ViewBag.statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = LangHelper.Get(HttpContext, "All"), Value = "", Selected = string.IsNullOrEmpty(status) },
                new SelectListItem { Text = LangHelper.Get(HttpContext, "Pending"), Value = "Pending", Selected = status == "Pending" },
                new SelectListItem { Text = LangHelper.Get(HttpContext, "Approved"), Value = "Approved", Selected = status == "Approved" },
                new SelectListItem { Text = LangHelper.Get(HttpContext, "Rejected"), Value = "Rejected", Selected = status == "Rejected" }
            };

            return View(requests);
        }
    }
}
