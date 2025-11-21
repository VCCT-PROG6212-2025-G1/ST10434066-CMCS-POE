using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }
        // get method for login
        [HttpGet]
        public IActionResult Login()
        {
            // go to home if already logged in
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }
        //post method for login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.UserAccounts
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "username or password invalid.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserID", user.UserAccountID);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", user.FullName);
            if (user.LecturerID.HasValue)
                HttpContext.Session.SetInt32("LecturerID", user.LecturerID.Value);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Denied()
        {
            return View();
        }
    }
}