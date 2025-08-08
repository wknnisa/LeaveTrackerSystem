using LeaveTrackerSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly LeaveTrackerDbContext _dbContext;
        public AdminController(
            LeaveTrackerDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            { 
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult AllRequests()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var allRequests = _dbContext.LeaveRequests.Include(r => r.User).Include(r => r.LeaveType).ToList();

            return View(allRequests);
        }
    }
}
