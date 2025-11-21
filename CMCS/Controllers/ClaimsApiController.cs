using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClaimsApiController(AppDbContext db)
        {
            _db = db;
        }

        // GET
        // status of claims 
        [HttpGet("stage")]
        public async Task<ActionResult<IEnumerable<object>>> GetByStage(string stage = "Pending")
        {
            if (!Enum.TryParse<ClaimStatus>(stage, true, out var status))
                return BadRequest("Stage not valid.");

            var claims = await _db.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.Year)
                .ThenByDescending(c => c.Month)
                .Select(c => new
                {
                    c.ClaimID,
                    LecturerName = c.Lecturer != null ? c.Lecturer.FullName : "Unknown",
                    c.Month,
                    c.Year,
                    c.HoursWorked,
                    c.HourlyRate,
                    Amount = c.Amount,
                    Status = c.Status.ToString()
                })
                .ToListAsync();

            return Ok(claims);
        }

        public class StatusChangeRequest
        {
            public int Id { get; set; }
            public string Decision { get; set; } = string.Empty; 
        }

        // POST
        
        [HttpPost("change")]
        public async Task<ActionResult> ChangeStatus([FromBody] StatusChangeRequest request)
        {
            var claim = await _db.Claims.FindAsync(request.Id);
            if (claim == null) return NotFound("Claim not found.");
            // the different types of status changes.
            switch (request.Decision.Trim().ToLower())
            {
                case "verify":
                    claim.Status = ClaimStatus.Verified;
                    break;
                case "approve":
                    claim.Status = ClaimStatus.Approved;
                    break;
                case "finalise":
                    claim.Status = ClaimStatus.Finalised;
                    break;
                case "reject":
                    claim.Status = ClaimStatus.Rejected;
                    break;
                default:
                    return BadRequest("must be: verify, approve, finalise, reject.");
            }

            await _db.SaveChangesAsync();
            return Ok(new { Message = $"Claim {claim.ClaimID} is now {claim.Status}." });
        }
    }
}