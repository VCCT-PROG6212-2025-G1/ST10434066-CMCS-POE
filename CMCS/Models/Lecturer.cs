using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Lecturer
    {
        public int LecturerID { get; set; }

        [Required, StringLength(20)] public string StaffNo { get; set; } = string.Empty;
        [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required, StringLength(100)] public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;

        [Range(0, 100000)]
        public decimal HourlyRate { get; set; }   

        public string FullName => $"{FirstName} {LastName}";
    }
}