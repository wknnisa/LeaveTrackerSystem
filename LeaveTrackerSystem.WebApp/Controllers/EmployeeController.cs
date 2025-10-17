using LeaveTrackerSystem.Application.DTOs;
using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.WebApp.Filters;
using LeaveTrackerSystem.WebApp.Helpers;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using LeaveTrackerSystem.WebApp.Services.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    [AuthorizeSession(Role = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(
            IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Submit()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var types = _employeeService.GetLeaveTypes();

            var model = new LeaveRequestViewModel
            {
                LeaveTypes = types.Select(t => new SelectListItem 
                { 
                    Text = t.Name,
                    Value = t.Id.ToString()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Submit(LeaveRequestViewModel model)
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            if (!ModelState.IsValid)
            {
                var types = _employeeService.GetLeaveTypes();

                model.LeaveTypes = types.Select(t => new SelectListItem
                    {
                        Text = t.Name,
                        Value = t.Id.ToString()
                    }).ToList();

                return View(model);
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;

            var dto = new LeaveRequestDto
            {
                LeaveTypeId = model.LeaveTypeId,
                StartDate = model.StartDate ?? DateTime.UtcNow,
                EndDate = model.EndDate ?? DateTime.UtcNow,
                Reason = model.Reason
            };

            var (success, message) = _employeeService.SubmitLeaveRequest(email, dto);

            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("MyRequests");
        }

        public IActionResult MyRequests(string? status)
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;

            var requests = _employeeService.GetMyRequests(email, status);

            ViewBag.statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "", Selected = string.IsNullOrEmpty(status) },
                new SelectListItem { Text = "Pending", Value = "Pending", Selected = status == "Pending" },
                new SelectListItem { Text = "Approved", Value = "Approved", Selected = status == "Approved" },
                new SelectListItem { Text = "Rejected", Value = "Rejected", Selected = status == "Rejected" }
            };

            return View(requests);
        }

        public IActionResult LeaveSummary()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;

            var summary = _employeeService.GetLeaveSummary(email);
            var usedLeaveByType = summary.ToDictionary(k => k.Key, v => v.Value.Used);
            var monthlyUsage = _employeeService.GetMonthlyUsage(email);

            ViewBag.LabelsJson = JsonConvert.SerializeObject(usedLeaveByType.Keys);
            ViewBag.DataJson = JsonConvert.SerializeObject(usedLeaveByType.Values);
            ViewBag.BarLabelsJson = JsonConvert.SerializeObject(monthlyUsage.Keys);
            ViewBag.BarDataJson = JsonConvert.SerializeObject(monthlyUsage.Values);

            return View(summary);
        }

        public IActionResult ExportSummary(string? status = null)
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;

            var summary = _employeeService.GetLeaveSummary(email);
            var pdfBytes = new LeavePdfService().GenerateLeaveRequestPdf(summary);

            return File(pdfBytes, "application/pdf", "LeaveSummary.pdf");
        }
    }
}
