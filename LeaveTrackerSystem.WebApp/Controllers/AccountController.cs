using LeaveTrackerSystem.WebApp.Models;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private static readonly List<User> seedUsers = new()
        {
            new User { Email = "admin@example.com", Password = "admin123", Role = "Admin" },
            new User { Email = "manager@example.com", Password = "manager123", Role = "Manager" },
            new User { Email = "employee@example.com", Password = "employee123", Role = "Employee" }
        };

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        { 
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = seedUsers.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase) 
            && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (user.Role == "Manager")
            {
                return RedirectToAction("Index", "Manager");
            }
            else
            {
                return RedirectToAction("Index", "Employee");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
