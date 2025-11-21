using CMCS.Data;
using CMCS.Models;
using CMCS.Models.Reports;  
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class HRController : Controller
    {
        private readonly AppDbContext _db;

        public HRController(AppDbContext db)
        {
            _db = db;
        }

        private bool IsHR()
        {
            return HttpContext.Session.GetString("Role") == "HR";
        }

        // GET: /HR
        public async Task<IActionResult> Index()
        {
            if (!IsHR()) return RedirectToAction("Denied", "Account");

            var users = await _db.UserAccounts
                .Include(u => u.Lecturer)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.Username)
                .ToListAsync();

            return View(users);
        }

        // GET: hr
        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsHR()) return RedirectToAction("Denied", "Account");

            var model = new UserAccountFormModel
            {
                Role = "Lecturer" 
            };
            return View(model);
        }

        // POST: hr
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserAccountFormModel model)
        {
            if (!IsHR()) return RedirectToAction("Denied", "Account");

            if (!ModelState.IsValid)
                return View(model);

            Lecturer? lecturer = null;

            if (model.Role == "Lecturer")
            {
                if (string.IsNullOrWhiteSpace(model.StaffNo) ||
                    string.IsNullOrWhiteSpace(model.LecturerFirstName) ||
                    string.IsNullOrWhiteSpace(model.LecturerLastName) ||
                    !model.HourlyRate.HasValue)
                {
                    ModelState.AddModelError("", "Fill in details and hourly rate.");
                    return View(model);
                }

                lecturer = new Lecturer
                {
                    StaffNo = model.StaffNo!,
                    FirstName = model.LecturerFirstName!,
                    LastName = model.LecturerLastName!,
                    Email = model.LecturerEmail ?? "",
                    HourlyRate = model.HourlyRate.Value
                };

                _db.Lecturers.Add(lecturer);
                await _db.SaveChangesAsync();
            }

            var fullName = model.FullName;
            if (model.Role == "Lecturer" && lecturer != null)
            {
                fullName = $"{lecturer.FirstName} {lecturer.LastName}";
            }

            var user = new UserAccount
            {
                Username = model.Username,
                Password = model.Password,
                FullName = fullName,
                Role = model.Role,
                LecturerID = lecturer?.LecturerID
            };

            _db.UserAccounts.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Successfully created.";
            return RedirectToAction(nameof(Index));
        }

        // GET:
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            if (!IsHR()) return RedirectToAction("Denied", "Account");

            var user = await _db.UserAccounts
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.UserAccountID == id);

            if (user == null) return NotFound();

            var model = new UserAccountFormModel
            {
                UserAccountID = user.UserAccountID,
                Username = user.Username,
                Password = user.Password,
                FullName = user.FullName,
                Role = user.Role,
                LecturerID = user.LecturerID
            };

            if (user.Role == "Lecturer" && user.Lecturer != null)
            {
                model.StaffNo = user.Lecturer.StaffNo;
                model.LecturerFirstName = user.Lecturer.FirstName;
                model.LecturerLastName = user.Lecturer.LastName;
                model.LecturerEmail = user.Lecturer.Email;
                model.HourlyRate = user.Lecturer.HourlyRate;
            }

            return View(model);
        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserAccountFormModel model)
        {
            if (!IsHR()) return RedirectToAction("Denied", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.UserAccounts
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.UserAccountID == model.UserAccountID);

            if (user == null) return NotFound();

            user.Username = model.Username;
            user.Password = model.Password;
            user.FullName = model.FullName;
            user.Role = model.Role;

            if (user.Role == "Lecturer")
            {
                if (user.Lecturer == null)
                {
                   
                    var newLecturer = new Lecturer
                    {
                        StaffNo = model.StaffNo ?? "",
                        FirstName = model.LecturerFirstName ?? "",
                        LastName = model.LecturerLastName ?? "",
                        Email = model.LecturerEmail ?? "",
                        HourlyRate = model.HourlyRate ?? 0
                    };
                    _db.Lecturers.Add(newLecturer);
                    await _db.SaveChangesAsync();

                    user.LecturerID = newLecturer.LecturerID;
                }
                else
                {
                    user.Lecturer.StaffNo = model.StaffNo ?? user.Lecturer.StaffNo;
                    user.Lecturer.FirstName = model.LecturerFirstName ?? user.Lecturer.FirstName;
                    user.Lecturer.LastName = model.LecturerLastName ?? user.Lecturer.LastName;
                    user.Lecturer.Email = model.LecturerEmail ?? user.Lecturer.Email;
                    if (model.HourlyRate.HasValue)
                        user.Lecturer.HourlyRate = model.HourlyRate.Value;
                }
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "successfully updated";
            return RedirectToAction(nameof(Index));
        }
        // GET: /HR/Reports
        //this generates reports and calculations. it sorts it by month and year.
        [HttpGet]
        public async Task<IActionResult> Reports(int? month, int? year)
        {
            if (!IsHR()) return RedirectToAction("Denied", "Account");

            var now = DateTime.UtcNow;
            var selectedMonth = month ?? now.Month;
            var selectedYear = year ?? now.Year;

            
            var claims = await _db.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatus.Finalised &&
                            c.Month == selectedMonth &&
                            c.Year == selectedYear)
                .ToListAsync();   

          
            var grouped = claims
                .GroupBy(c => new
                {
                    c.LecturerID,
                    Name = c.Lecturer != null
                        ? c.Lecturer.FullName
                        : "Unknown"
                })
                .Select(g => new ClaimSummaryItem
                {
                    LecturerID = g.Key.LecturerID,
                    LecturerName = g.Key.Name,
                    TotalHours = g.Sum(x => x.HoursWorked),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderBy(i => i.LecturerName)
                .ToList();

            var model = new ClaimReportViewModel
            {
                Month = selectedMonth,
                Year = selectedYear,
                Items = grouped
            };

            return View(model);
        }
    }
}