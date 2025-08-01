using LeaveTrackerSystem.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            return role switch
            {
                "Employee" => RedirectToAction("Dashboard", "Employee"),
                "Manager" => RedirectToAction("Dashboard", "Manager"),
                "Admin" => RedirectToAction("AllRequests", "Admin"),
                _=> RedirectToAction("Login", "Account")
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
