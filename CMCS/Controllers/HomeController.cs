using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
