using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Mock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult AllRequests(string? status)
        {
            if (HttpContext.Session.GetString("Role") != "Manager")
            {
                return RedirectToAction("Login", "Account");
            }

            var email = HttpContext.Session.GetString("Email");
            var requests = InMemoryData.LeaveRequests.Where(r => r.Email != email).ToList();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                requests = requests.Where(r => r.Status == parsed).ToList();
            }

            requests = requests.OrderBy(r => (int)r.Status).ToList();

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
            var request = InMemoryData.LeaveRequests.FirstOrDefault(r => r.Id == id);

            if (request != null && request.Status == LeaveStatus.Pending)
            {
                request.Status = LeaveStatus.Approved;
            }

            return RedirectToAction("AllRequests");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var request = InMemoryData.LeaveRequests.FirstOrDefault(r => r.Id == id);

            if (request != null && request.Status == LeaveStatus.Pending)
            {
                request.Status = LeaveStatus.Rejected;
            }

            return RedirectToAction("AllRequests");
        }
    }
}
