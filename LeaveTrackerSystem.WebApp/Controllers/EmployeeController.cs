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

            // TEMP: Simulating successful submission - no database logic yet
            TempData["Success"] = "Leave request submitted successfully.";
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
    }
}
