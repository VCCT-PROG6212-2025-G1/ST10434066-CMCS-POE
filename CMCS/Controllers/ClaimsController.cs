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
            ViewBag.Lecturers = _db.Lecturers.OrderBy(l => l.FirstName).ToList();
            return View(new Claim { Month = DateTime.UtcNow.Month, Year = DateTime.UtcNow.Year });
        }

        // POST: /Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, IFormFile? upload)
        {
            var lecturer = await _db.Lecturers.FindAsync(claim.LecturerID);
            if (lecturer == null) ModelState.AddModelError("LecturerID", "Invalid Lecturer.");
            if (claim.Month is < 1 or > 12) ModelState.AddModelError("Month", "Month invalid.");
            if (claim.Year is < 2000 or > 2100) ModelState.AddModelError("Year", "Invalid year.");
            if (claim.HoursWorked <= 0) ModelState.AddModelError("HoursWorked", "Hours must be > 0.");

            if (!ModelState.IsValid)
            {
                ViewBag.Lecturers = _db.Lecturers.OrderBy(l => l.FirstName).ToList();
                return View(claim);
            }

            // Store the lecturer's rate at time of claim
            claim.HourlyRate = lecturer!.HourlyRate;
            claim.Status = ClaimStatus.Pending;

            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            // Optional upload
            if (upload != null && upload.Length > 0)
            {
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };
                var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
                var maxBytes = 5 * 1024 * 1024;

                if (!allowed.Contains(ext))
                    TempData["UploadError"] = "Invalid file type.";
                else if (upload.Length > maxBytes)
                    TempData["UploadError"] = "File > 5 MB.";
                else
                {
                    try
                    {
                        var root = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                        Directory.CreateDirectory(root);
                        var saved = $"{Guid.NewGuid()}{ext}";
                        var path = Path.Combine(root, saved);
                        using var fs = new FileStream(path, FileMode.Create);
                        await upload.CopyToAsync(fs);

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
                        TempData["UploadError"] = "Upload failed.";
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