using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using LeaveTrackerSystem.WebApp.Services.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly LeaveBalanceService _leaveBalanceService;
        private readonly LeaveTrackerDbContext _dbContext;

        public EmployeeController(
            LeaveBalanceService leaveBalanceService,
            LeaveTrackerDbContext dbContext)
        {
            _leaveBalanceService = leaveBalanceService;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Employee")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Submit()
        {
            var model = new LeaveRequestViewModel
            {
                LeaveTypes = GetLeaveTypeOptions()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Submit(LeaveRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.LeaveTypes = GetLeaveTypeOptions();
                return View(model);
            }

            var email = HttpContext.Session.GetString("Email");
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login", "Account");
            }

            var leaveType = _dbContext.LeaveTypes.FirstOrDefault(x => x.Id == model.LeaveTypeId);

            if (leaveType == null)
            {
                TempData["Error"] = "Leave type not found.";
                return RedirectToAction("Submit");
            }

            var daysRequested = (model.EndDate - model.StartDate).TotalDays  + 1;
            
            if (daysRequested > leaveType.DefaultDays) 
            {
                TempData["Error"] = "Requested days exceed your leave entitlement.";
                model.LeaveTypes = GetLeaveTypeOptions() ;
                return View(model);
            }

            var request = new LeaveRequest
            {
                UserId = user.Id,
                LeaveTypeId = leaveType.Id,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Reason = model.Reason,
                Status = LeaveStatus.Pending,
                RequestedAt = DateTime.Now,
            };

            _dbContext.LeaveRequests.Add(request);
            _dbContext.SaveChanges();

            TempData["Success"] = "Leave request submitted successfully.";
            return RedirectToAction("Submit");
        }

        public IActionResult LeaveSummary()
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login", "Account");
            }

            var summary = _dbContext.LeaveTypes
                .Select(x => new
                {
                    TypeName = x.Name,
                    Used = _dbContext.LeaveRequests.Count(lr => lr.UserId == user.Id && lr.LeaveTypeId == x.Id && lr.Status == LeaveStatus.Approved),
                    Remaining = x.DefaultDays - _dbContext.LeaveRequests.Where(lr => lr.UserId == user.Id && lr.LeaveTypeId == x.Id && lr.Status == LeaveStatus.Approved)
                                .Sum(lr => EF.Functions.DateDiffDay(lr.StartDate, lr.EndDate) + 1)
                }).ToDictionary(k => k.TypeName, v => (v.Used, v.Remaining < 0 ? 0 : v.Remaining));

            var usedLeaveByType = summary.ToDictionary(k => k.Key, v => v.Value.Used);

            ViewBag.PieChartData = usedLeaveByType;
            ViewBag.LabelsJson = JsonConvert.SerializeObject(usedLeaveByType.Keys);
            ViewBag.DataJson = JsonConvert.SerializeObject(usedLeaveByType.Values);

            var monthlyUsage = _dbContext.LeaveRequests
                .Where(r => r.UserId == user.Id && r.Status == LeaveStatus.Approved)
                .GroupBy(r => new DateTime(r.StartDate.Year, r.StartDate.Month, 1))
                .AsEnumerable()
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => g.Key.ToString("MM/yyyy"),
                    g => g.Count()
                );

            ViewBag.BarLabelsJson = JsonConvert.SerializeObject(monthlyUsage.Keys);
            ViewBag.BarDataJson = JsonConvert.SerializeObject(monthlyUsage.Values);

            return View(summary);
        }

        private List<SelectListItem> GetLeaveTypeOptions()
        {
            return _dbContext.LeaveTypes
            .Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString()
            }).ToList();
        }

        public IActionResult MyRequests(string? status)
        { 
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login", "Account");
            }

            var myRequestsQuery = _dbContext.LeaveRequests.Include(r => r.LeaveType).Where(r => r.UserId == user.Id).AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                myRequestsQuery = myRequestsQuery.Where(r => r.Status == parsed);
            }

            var myRequests = myRequestsQuery.OrderBy(r => (int)r.Status).ToList();

            ViewBag.statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "", Selected = string.IsNullOrEmpty(status) },
                new SelectListItem { Text = "Pending", Value = "Pending", Selected = status == "Pending" },
                new SelectListItem { Text = "Approved", Value = "Approved", Selected = status == "Approved" },
                new SelectListItem { Text = "Rejected", Value = "Rejected", Selected = status == "Rejected" }
            };

            return View(myRequests);
        }

        public IActionResult ExportSummary(string? status = null)
        {
            var email = HttpContext.Session.GetString("Email") ?? "unknown@example.com";

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            LeaveStatus? statusFilter = null;

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, true, out var parsedStatus))
            {
                statusFilter = parsedStatus;
            }

            var summary = _leaveBalanceService.GetLeaveSummary(email, statusFilter);
            var pdfBytes = new LeavePdfService().GenerateLeaveRequestPdf(summary);

            return File(pdfBytes, "application/pdf", "LeaveSummary.pdf");
        }
    }
}
