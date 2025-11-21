using CMCS.Data;
using CMCS.Models;
using CMCS.Models.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HrApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HrApiController(AppDbContext db)
        {
            _db = db;
        }

        // GET
        [HttpGet("finalised")]
        public async Task<ActionResult<IEnumerable<ClaimSummaryItem>>> GetFinalisedReport(int? month, int? year)
        {
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
                    Name = c.Lecturer != null ? c.Lecturer.FullName : "Unknown"
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

            return Ok(grouped);
        }
    }
}
