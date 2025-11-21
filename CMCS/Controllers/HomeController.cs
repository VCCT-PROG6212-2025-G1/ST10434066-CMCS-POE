using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
                return RedirectToAction("Login", "Account");

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}