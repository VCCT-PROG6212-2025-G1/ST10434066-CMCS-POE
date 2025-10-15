using CMCS.Models;
using Xunit;

namespace CMCS.Tests;

public class SubmissionFlowTest
{
    [Fact]
    public void SubmitTest()
    {
        var lecturerTest = new Lecturer
        {
            StaffNo = "L5",
            FirstName = "Sameeha",
            LastName = "Khan",
            Email = "khan@uni.ac.za",
            HourlyRate = 450
        };

        var testClaim = new Claim
        {
            LecturerID = 5,
            Month = 6,
            Year = 2026,
            HoursWorked = 11,
            HourlyRate = lecturerTest.HourlyRate,
            Status = ClaimStatus.Pending
        };

        Assert.Equal(ClaimStatus.Pending, testClaim.Status);
        Assert.Equal(450, testClaim.HourlyRate);
        Assert.Equal(4950, testClaim.Amount);
    }
}