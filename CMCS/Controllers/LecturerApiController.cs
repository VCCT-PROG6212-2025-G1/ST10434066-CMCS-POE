using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LecturerApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LecturerApiController(AppDbContext db)
        {
            _db = db;
        }

        // GET
        // this returns the lecturer details for the given id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetLecturer(int id)
        {
            var lecturer = await _db.Lecturers.FindAsync(id);
            if (lecturer == null) return NotFound();

            return Ok(new
            {
                lecturer.LecturerID,
                lecturer.StaffNo,
                lecturer.FullName,
                lecturer.Email,
                lecturer.HourlyRate
            });
        }

        // GET
        // auto calculation
        [HttpGet("calc")]
        public async Task<ActionResult<object>> CalculateAmount(int lecturerId, decimal hours)
        {
            var lecturer = await _db.Lecturers.FindAsync(lecturerId);
            if (lecturer == null) return NotFound("No lecturer.");

            if (hours < 0) return BadRequest("Hours must be >= 0.");

            var rate = lecturer.HourlyRate;
            var amount = hours * rate;

            return Ok(new
            {
                LecturerID = lecturer.LecturerID,
                LecturerName = lecturer.FullName,
                Hours = hours,
                HourlyRate = rate,
                Amount = amount
            });
        }
    }
}