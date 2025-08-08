using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly LeaveTrackerDbContext _dbContext;

        public ManagerController(
            LeaveTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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
            var role = HttpContext.Session.GetString("Role");
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (currentUser == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login", "Account");
            }

            var requestsQuery = _dbContext.LeaveRequests.Include(r => r.LeaveType).Include(r => r.User).AsQueryable();

            if (role == "Manager")
            {
                requestsQuery = requestsQuery.Where(r => r.UserId != currentUser.Id);
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                requestsQuery = requestsQuery.Where(r => r.Status == parsed);
            }

            var requests = requestsQuery.OrderBy(r => (int)r.Status).ToList();

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
            var request = _dbContext.LeaveRequests.FirstOrDefault(r => r.Id == id);

            if (request != null && request.Status == LeaveStatus.Pending)
            {
                request.Status = LeaveStatus.Approved;
                _dbContext.SaveChanges();
            }

            return RedirectToAction("AllRequests");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var request = _dbContext.LeaveRequests.FirstOrDefault(r => r.Id == id);

            if (request != null && request.Status == LeaveStatus.Pending)
            {
                request.Status = LeaveStatus.Rejected;
                _dbContext.SaveChanges();
            }

            return RedirectToAction("AllRequests");
        }
    }
}
