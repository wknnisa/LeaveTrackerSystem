using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using LeaveTrackerSystem.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly LeaveTrackerDbContext _dbContext;

        public AccountController(LeaveTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role.ToString());

            return user.Role switch
            {
                RoleEnum.Admin => RedirectToAction("Index", "Admin"),
                RoleEnum.Manager => RedirectToAction("Index", "Manager"),
                _ => RedirectToAction("Index", "Employee")
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
