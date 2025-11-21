using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ClaimsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db; _env = env;
        }

        // Claims Create
        public IActionResult Create()
        {
            // Only lecturers submit claims
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Denied", "Account");

            var lecturerId = HttpContext.Session.GetInt32("LecturerID");
            if (lecturerId == null)
                return RedirectToAction("Login", "Account");

            var lecturer = _db.Lecturers.Find(lecturerId.Value);
            if (lecturer == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Lecturer = lecturer;

            var model = new Claim
            {
                LecturerID = lecturer.LecturerID,
                Month = DateTime.UtcNow.Month,
                Year = DateTime.UtcNow.Year
            };

            return View(model);
        }

        // POST: /Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, IFormFile? upload)
        {
            // Only lecturers  submit claims
            if (HttpContext.Session.GetString("Role") != "Lecturer")
                return RedirectToAction("Denied", "Account");

            var lecturerId = HttpContext.Session.GetInt32("LecturerID");
            if (lecturerId == null)
                return RedirectToAction("Login", "Account");

            var lecturer = await _db.Lecturers.FindAsync(lecturerId.Value);
            if (lecturer == null)
                return RedirectToAction("Login", "Account");

            // Force claim to belong to the logged-in lecturer
            claim.LecturerID = lecturer.LecturerID;

            // ========= VALIDATION =========
            if (claim.Month is < 1 or > 12)
                ModelState.AddModelError("Month", "Month invalid.");

            if (claim.Year is < 2000 or > 2100)
                ModelState.AddModelError("Year", "Invalid year.");

            if (claim.HoursWorked <= 0)
                ModelState.AddModelError("HoursWorked", "Hours must be > 0.");

            if (claim.HoursWorked > 180)
                ModelState.AddModelError("HoursWorked", "hours must be < 180 a month.");

            if (!ModelState.IsValid)
            {
                ViewBag.Lecturer = lecturer;
                return View(claim);
            }

            // hourly rate from lecturer record
            claim.HourlyRate = lecturer.HourlyRate;        
            claim.Status = ClaimStatus.Pending;

            // save claim to get ClaimID
            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();          

            // file upload handling
            if (upload != null && upload.Length > 0)
            {
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };
                var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
                var maxBytes = 5 * 1024 * 1024; // this is 5mb 

                if (!allowed.Contains(ext))
                {
                    TempData["UploadError"] = "Invalid file type.";
                }
                else if (upload.Length > maxBytes)
                {
                    TempData["UploadError"] = "File size must be < 5 MB.";
                }
                else
                {
                    // save the file
                    try
                    {
                        var root = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                        Directory.CreateDirectory(root);

                        var saved = $"{Guid.NewGuid()}{ext}";
                        var path = Path.Combine(root, saved);

                        using (var fs = new FileStream(path, FileMode.Create))
                        {
                            await upload.CopyToAsync(fs);
                        }

                        _db.SupportingDocuments.Add(new SupportingDocument
                        {
                            ClaimID = claim.ClaimID,
                            FileName = upload.FileName,
                            FileUrl = $"/uploads/{saved}"
                        });

                        await _db.SaveChangesAsync();
                    }
                    catch
                    {
                        TempData["UploadError"] = "Upload failed. Please try again.";
                    }
                }
            }

            TempData["Success"] = "Claim submitted successfully.";
            return RedirectToAction(nameof(Index));
        }
        //Claims
        public async Task<IActionResult> Index()
        {
            var items = await _db.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.Year).ThenByDescending(c => c.Month)
                .ToListAsync();
            return View(items);
        }
    }
}