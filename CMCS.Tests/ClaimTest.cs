using CMCS.Models;
using Xunit;

namespace CMCS.Tests;
// test to calculate the amount of a claim based on hours worked and hourly rate.
public class ClaimTests
{
    [Fact]
    public void Amount()
    {
        var test = new Claim { HoursWorked = 12, HourlyRate = 500 };
        Assert.Equal(6000, test.Amount);
    }
}