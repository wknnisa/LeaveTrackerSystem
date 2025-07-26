using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private static readonly List<LeaveRequest> LeaveRequests = new();
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
            var leaveTypeName = model.LeaveTypes?.FirstOrDefault(l => l.Value == model.LeaveTypeId.ToString())?.Text ?? "Unknown";

            LeaveRequests.Add(new LeaveRequest
            {
                Email = email ?? "unknown@example.com",
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                LeaveType = leaveTypeName,
                Reason = model.Reason,
                Status = LeaveStatus.Pending
            });

            // TEMP: Simulating successful submission - no database logic yet
            TempData["Success"] = "Leave request submitted successfully and marked as Pending.";
            return RedirectToAction("Submit");
        }

        private List<SelectListItem> GetLeaveTypeOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Annual Leave" },
                new SelectListItem { Value = "2", Text = "Medical Leave" },
                new SelectListItem { Value = "3", Text = "Emergency Leave" }
            };
        }

        public IActionResult MyRequests()
        { 
            var email = HttpContext.Session.GetString("Email");
            var myRequests = LeaveRequests.Where(r => r.Email == email).ToList();

            return View(myRequests);
        }
    }
}
