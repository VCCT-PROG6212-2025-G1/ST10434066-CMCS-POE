using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly AppDbContext _db;

        public ApprovalsController(AppDbContext db)
        {
            _db = db;
        }

        private string? CurrentRole => HttpContext.Session.GetString("Role");

        // GET: Pending
        public async Task<IActionResult> Pending()
        {
            if (string.IsNullOrEmpty(CurrentRole))
                return RedirectToAction("Login", "Account");

            IQueryable<Claim> query = _db.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.Year)
                .ThenByDescending(c => c.Month);
            // Role based filetering and verification.
            switch (CurrentRole)
            {
                case "Coordinator":
                    
                    query = query.Where(c => c.Status == ClaimStatus.Pending);
                    break;

                case "Manager":
                  
                    query = query.Where(c => c.Status == ClaimStatus.Verified);
                    break;

                case "HR":
                   
                    query = query.Where(c => c.Status == ClaimStatus.Approved);
                    break;

                default:
                    return RedirectToAction("Denied", "Account");
            }

            ViewBag.Role = CurrentRole;
            var list = await query.ToListAsync();
            return View(list);
        }

        // Coordinator: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id)
        {
            if (CurrentRole != "Coordinator")
                return RedirectToAction("Denied", "Account");

            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            if (claim.Status == ClaimStatus.Pending)
            {
                claim.Status = ClaimStatus.Verified;
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Claim {id} verified.";
            }

            return RedirectToAction(nameof(Pending));
        }

        // Manager: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (CurrentRole != "Manager")
                return RedirectToAction("Denied", "Account");

            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            if (claim.Status == ClaimStatus.Verified)
            {
                claim.Status = ClaimStatus.Approved;
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Claim {id} approved.";
            }

            return RedirectToAction(nameof(Pending));
        }

        // HR: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalise(int id)
        {
            if (CurrentRole != "HR")
                return RedirectToAction("Denied", "Account");

            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            if (claim.Status == ClaimStatus.Approved)
            {
                claim.Status = ClaimStatus.Finalised;
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Claim {id} finalised.";
            }

            return RedirectToAction(nameof(Pending));
        }
    }
}