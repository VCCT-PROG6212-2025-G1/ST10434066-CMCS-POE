using CMCS.Data;
using CMCS.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CMCS.Tests;
// Tests for approving and rejecting claims.
public class TestApproval
{
    private static Claim SeedPendingClaim(AppDbContext db)
    {
        var testlecturer = new Lecturer { StaffNo = "L2", FirstName = "Fatima", LastName = "Parker", Email = "parker@uni.co.za", HourlyRate = 500 };
        db.Lecturers.Add(testlecturer);
        db.SaveChanges();

        var testClaim = new Claim
        {
            LecturerID = testlecturer.LecturerID,
            Month = 10,
            Year = 2025,
            HoursWorked = 8,
            HourlyRate = testlecturer.HourlyRate,
            Status = ClaimStatus.Pending
        };
        db.Claims.Add(testClaim);
        db.SaveChanges();
        return testClaim;
    }

    [Fact]
    public void Approve_Sets_Status_Approved()
    {
        using var db = TestHelpers.NewInMemoryDb();
        var testClaim = SeedPendingClaim(db);

        testClaim.Status = ClaimStatus.Approved;
        db.SaveChanges();

        Assert.Equal(ClaimStatus.Approved, db.Claims.Single().Status);
    }

    [Fact]
    public void Reject_Sets_Status_Rejected()
    {
        using var db = TestHelpers.NewInMemoryDb();
        var testClaim = SeedPendingClaim(db);

        testClaim.Status = ClaimStatus.Rejected;
        db.SaveChanges();

        Assert.Equal(ClaimStatus.Rejected, db.Claims.Single().Status);
    }
}