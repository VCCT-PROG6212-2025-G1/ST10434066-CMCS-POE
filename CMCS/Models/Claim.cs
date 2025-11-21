using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public enum ClaimStatus
    {
        Pending = 0,   // When lecturer submits
        Verified = 1,  // Coordinator has verified
        Approved = 2,  // Manager has approved
        Finalised = 3, // HR finalised payment
        Rejected = 4   // Any role can reject
    }

    public class Claim
    {
        public int ClaimID { get; set; }

        [Required]
        public int LecturerID { get; set; }
        public Lecturer? Lecturer { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        [Range(0, 1000)]
        public decimal HoursWorked { get; set; }

        [Range(0, 100000)]
        public decimal HourlyRate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public List<SupportingDocument> Documents { get; set; } = new();

        [NotMapped]
        public decimal Amount => Math.Round(HoursWorked * HourlyRate, 2);
    }
}