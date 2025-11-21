using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
	public class UserAccount
	{
		public int UserAccountID { get; set; }

		[Required, StringLength(50)]
		public string Username { get; set; } = string.Empty;

		[Required, StringLength(100)]
		public string Password { get; set; } = string.Empty; 

		[Required, StringLength(100)]
		public string FullName { get; set; } = string.Empty;

		
		[Required, StringLength(20)]
		public string Role { get; set; } = string.Empty;

		
		public int? LecturerID { get; set; }
		public Lecturer? Lecturer { get; set; }
	}
}