using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Mock;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static LeaveTrackerSystem.Infrastructure.Mock.InMemoryData;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly LeaveBalanceService _leaveBalanceService;

        public EmployeeController(LeaveBalanceService leaveBalanceService)
        {
            _leaveBalanceService = leaveBalanceService;
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

            var daysRequested = (model.EndDate - model.StartDate).TotalDays  + 1;

            //var email = HttpContext.Session.GetString("Email");
            //var leaveTypeName = Enum.IsDefined(typeof(LeaveType), model.LeaveTypeId) ? ((LeaveType)model.LeaveTypeId).ToString() : "Unknown";

            var leaveType = (LeaveType)model.LeaveTypeId;
            var email = HttpContext.Session.GetString("Email") ?? "unknown@example.com";

            if (!LeaveBalanceStore.UsedLeave.ContainsKey(email))
            {
                LeaveBalanceStore.UsedLeave[email] = new Dictionary<LeaveType, int>();
            }

            if (!LeaveBalanceStore.UsedLeave[email].ContainsKey(leaveType))
            {
                LeaveBalanceStore.UsedLeave[email][leaveType] = 0;
            }

            var used = LeaveBalanceStore.UsedLeave[email][leaveType];
            var max = LeaveBalanceStore.MaxBalance.ContainsKey(leaveType)? LeaveBalanceStore.MaxBalance[leaveType]:0 ;

            if (used + daysRequested > max) 
            {
                TempData["Error"] = "Insufficient leave balance for this leave type.";
                model.LeaveTypes = GetLeaveTypeOptions() ;
                return View(model);
            }

            int nextId = InMemoryData.LeaveRequests.Any() ? InMemoryData.LeaveRequests.Max(r => r.Id) + 1 : 1;

            InMemoryData.LeaveRequests.Add(new LeaveRequest
            {
                Id = nextId,
                Email = email,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                LeaveType = leaveType,
                Reason = model.Reason,
                Status = LeaveStatus.Pending
            });

            LeaveBalanceStore.UsedLeave[email][leaveType] += (int)daysRequested;

            // TEMP: Simulating successful submission - no database logic yet
            TempData["Success"] = "Leave request submitted successfully and marked as Pending.";
            return RedirectToAction("Submit");
        }

        public IActionResult LeaveSummary()
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var summary = new Dictionary<string, (int used, int remaining)>();

            foreach (LeaveType type in Enum.GetValues(typeof(LeaveType)))
            {
                int used = _leaveBalanceService.GetUsedLeaveDays(email, type);

                int remaining = _leaveBalanceService.GetRemainingLeave(email, type);

                summary[type.ToString()] = (used, remaining);
            }

            return View(summary);
        }

        private List<SelectListItem> GetLeaveTypeOptions()
        {
            return Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>().Where(e => e != LeaveType.Unknown)
            .Select(e => new SelectListItem
            {
                Text = e.ToString(),
                Value = ((int)e).ToString()
            }).ToList();
        }

        public IActionResult MyRequests(string? status)
        { 
            var email = HttpContext.Session.GetString("Email");
            var myRequests = InMemoryData.LeaveRequests.Where(r => r.Email == email).ToList();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                myRequests = myRequests.Where(r => r.Status == parsed).ToList();
            }

            myRequests = myRequests.OrderBy(r => (int)r.Status).ToList();

            ViewBag.statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "", Selected = string.IsNullOrEmpty(status) },
                new SelectListItem { Text = "Pending", Value = "Pending", Selected = status == "Pending" },
                new SelectListItem { Text = "Approved", Value = "Approved", Selected = status == "Approved" },
                new SelectListItem { Text = "Rejected", Value = "Rejected", Selected = status == "Rejected" }
            };

            return View(myRequests);
        }
    }
}
