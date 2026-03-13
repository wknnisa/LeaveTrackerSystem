using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using LeaveTrackerSystem.WebApp.Helpers;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly LeaveTrackerDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;

        public AccountController(LeaveTrackerDbContext dbContext, 
            ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? msg)
        {
            if (msg == "expired")
            {
                TempData["Info"] = "Your session has expired. Please log in again.";
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        { 
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login attempt failed due to invalid model state for email {Email}", model.Email);

                TempData["Error"] = "Please fill in all required fields.";
                return View(model);
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for email {Email}", model.Email);

                TempData["Error"] = "Invalid email or password.";
                return View(model);
            }

            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role.ToString());

            _logger.LogInformation("User {Email} logged in with role {Role}", user.Email, user.Role);

            TempData["Success"] = $"Welcome back, {user.Role}";

            return user.Role switch
            {
                RoleEnum.Admin => RedirectToAction("Index", "Admin"),
                RoleEnum.Manager => RedirectToAction("Index", "Manager"),
                _ => RedirectToAction("Index", "Employee")
            };
        }

        public IActionResult Logout()
        {
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Account", new { msg = "expired" });
            }

            TempData["Info"] = LangHelper.Get(HttpContext, "LogoutSuccess");

            _logger.LogInformation("User {Email} with role {Role} logged out", email ?? "Unknown", role ?? "Unknown");

            HttpContext.Session.Clear();
            
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult CheckSession()
        {
            if (!SessionHelper.IsSessionActive(HttpContext))
            {
                return StatusCode(440);
            }

            return Ok();
        }
    }
}
