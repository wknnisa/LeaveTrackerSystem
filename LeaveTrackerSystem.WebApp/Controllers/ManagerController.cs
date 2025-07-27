using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Mock;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult PendingRequests()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Manager" && role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var pending = InMemoryData.LeaveRequests.Where(r => r.Status == LeaveStatus.Pending).ToList();

            return View(pending);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var request = InMemoryData.LeaveRequests.FirstOrDefault(r => r.Id == id);

            if (request != null && request.Status == LeaveStatus.Pending)
            {
                request.Status = LeaveStatus.Approved;
            }

            return RedirectToAction("PendingRequests");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var request = InMemoryData.LeaveRequests.FirstOrDefault(r => r.Id == id);

            if (request != null && request.Status == LeaveStatus.Pending)
            {
                request.Status = LeaveStatus.Rejected;
            }

            return RedirectToAction("PendingRequests");
        }
    }
}
