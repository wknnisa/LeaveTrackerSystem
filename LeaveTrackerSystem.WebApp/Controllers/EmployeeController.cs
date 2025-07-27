using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Mock;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class EmployeeController : Controller
    {
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
            var leaveTypeName = Enum.IsDefined(typeof(LeaveType), model.LeaveTypeId) ? ((LeaveType)model.LeaveTypeId).ToString() : "Unknown";

            InMemoryData.LeaveRequests.Add(new LeaveRequest
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
            return Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>().Where(e => e != LeaveType.Unknown)
            .Select(e => new SelectListItem
            {
                Text = e.ToString(),
                Value = ((int)e).ToString()
            }).ToList();
        }

        public IActionResult MyRequests()
        { 
            var email = HttpContext.Session.GetString("Email");
            var myRequests = InMemoryData.LeaveRequests.Where(r => r.Email == email).ToList();

            return View(myRequests);
        }
    }
}
