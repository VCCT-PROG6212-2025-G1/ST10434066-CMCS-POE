using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class SupportingDocument
    {
        public int SupportingDocumentID { get; set; }

        [Required] public int ClaimID { get; set; }
        public Claim? Claim { get; set; }

        [Required, StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required, StringLength(512)]
        public string FileUrl { get; set; } = string.Empty;

        public DateTime Uploaded { get; set; } = DateTime.UtcNow;
    }
}