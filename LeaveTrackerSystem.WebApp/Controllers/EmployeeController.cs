using LeaveTrackerSystem.Application.DTOs;
using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
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
        private readonly INotificationService _notificationService;

        public EmployeeController(
            IEmployeeService employeeService, 
            INotificationService notificationService)
        {
            _employeeService = employeeService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;
            var summary = _employeeService.GetLeaveSummary(email);
            var requests = _employeeService.GetMyRequests(email, null).ToList();
            
            var approved = requests.Count(r => r.Status == LeaveStatus.Approved);
            var pending = requests.Count(r => r.Status == LeaveStatus.Pending);
            var rejected = requests.Count(r => r.Status == LeaveStatus.Rejected);
            var total = requests.Count;

            var totalLeave = summary.Values.Sum(v => v.Remaining + v.Used);
            var usedLeave = summary.Values.Sum(v => v.Used);
            var remainingLeave = totalLeave - usedLeave;

            var usedLeaveByType = summary.ToDictionary(k => k.Key, v => v.Value.Used);
            ViewBag.LabelsJson = JsonConvert.SerializeObject(usedLeaveByType.Keys);
            ViewBag.DataJson = JsonConvert.SerializeObject(usedLeaveByType.Values);

            var hasPieData = usedLeaveByType.Values.Any(v => v > 0);
            ViewBag.HasPieData = hasPieData;

            ViewBag.TotalRequests = total;
            ViewBag.Approved = approved;
            ViewBag.Pending = pending;
            ViewBag.Rejected = rejected;
            ViewBag.LeaveBalance = $"{remainingLeave}/{totalLeave}";
            ViewBag.Name = HttpContext.Session.GetString("Name") ?? HttpContext.Session.GetString("Role");

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
        [ValidateAntiForgeryToken]
        public IActionResult Submit(LeaveRequestViewModel model)
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;

            if (!ModelState.IsValid)
            {
                PopulateLeaveTypes(model);
                return View(model);
            }

            var startDate = model.StartDate!.Value;
            var endDate = model.EndDate!.Value;

            var dto = new LeaveRequestDto
            {
                LeaveTypeId = model.LeaveTypeId,
                StartDate = startDate,
                EndDate = endDate,
                Reason = model.Reason
            };

            var (success, message) = _employeeService.SubmitLeaveRequest(email, dto);

            if (success)
            {
                var types = _employeeService.GetLeaveTypes();
                var selectedType = types.FirstOrDefault(t => t.Id == model.LeaveTypeId);

                var request = new LeaveRequest
                {
                    LeaveType = new LeaveType
                    {
                        Id = selectedType?.Id ?? 0,
                        Name = selectedType?.Name ?? LangHelper.Get(HttpContext, "Unknown")
                    },
                    StartDate = startDate,
                    EndDate = endDate
                };

                _notificationService.NotifyLeaveSubmission(email, request);
                TempData["Info"] = LangHelper.Get(HttpContext, "NotificationSimulated");
            }

            TempData[success ? "Success" : "Error"] = success ? LangHelper.Get(HttpContext, "LeaveSubmitSuccess") : LangHelper.Get(HttpContext, "LeaveSubmitFail");
            return RedirectToAction("MyRequests");
        }

        private void PopulateLeaveTypes(LeaveRequestViewModel model)
        {
            var types = _employeeService.GetLeaveTypes();

            model.LeaveTypes = types.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
        }

        public IActionResult MyRequests(string? status, int page = 1)
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            var email = SessionHelper.GetUserEmail(HttpContext)!;

            int pageSize = 10;

            var result = _employeeService.GetMyRequests(email, status, page, pageSize);

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

            var pieDataList = usedLeaveByType.Values.ToList();
            var barDataList = monthlyUsage.Values.ToList();

            ViewBag.LabelsJson = JsonConvert.SerializeObject(usedLeaveByType.Keys);
            ViewBag.DataJson = JsonConvert.SerializeObject(usedLeaveByType.Values);
            ViewBag.BarLabelsJson = JsonConvert.SerializeObject(monthlyUsage.Keys);
            ViewBag.BarDataJson = JsonConvert.SerializeObject(monthlyUsage.Values);

            ViewBag.HasPieData = pieDataList.Any(x => x > 0);
            ViewBag.HasBarData = barDataList.Any(x => x > 0);

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
