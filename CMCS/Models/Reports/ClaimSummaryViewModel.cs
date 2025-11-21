namespace CMCS.Models.Reports
{
    public class ClaimSummaryItem
    {
        public int LecturerID { get; set; }
        public string LecturerName { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ClaimReportViewModel
    {
        public int? Month { get; set; }
        public int? Year { get; set; }

        public List<ClaimSummaryItem> Items { get; set; } = new();
        public decimal GrandTotalHours => Items.Sum(i => i.TotalHours);
        public decimal GrandTotalAmount => Items.Sum(i => i.TotalAmount);
    }
}