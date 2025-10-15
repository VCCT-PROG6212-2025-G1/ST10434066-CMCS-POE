using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly AppDbContext _db;
        public ApprovalsController(AppDbContext db) { _db = db; }

        // pending approvals
        public async Task<IActionResult> Pending()
        {
            var claims = await _db.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.Year).ThenByDescending(c => c.Month)
                .ToListAsync();

            return View(claims);
        }

        // approved or not 
        [HttpPost]
        public async Task<IActionResult> Decide(int id, string decision)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = decision == "Approve" ? ClaimStatus.Approved : ClaimStatus.Rejected;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Claim {id} {claim.Status}";
            return RedirectToAction(nameof(Pending));
        }
    }
}