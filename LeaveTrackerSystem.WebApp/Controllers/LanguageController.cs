using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.WebApp.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult SwitchLanguage(string lang)
        {
            if (lang != "EN" && lang != "BM")
            {
                lang = "EN";
            }

            HttpContext.Session.SetString("Lang", lang);

            var referer = Request.Headers["Referer"].ToString();

            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(referer);
        }
    }
}
