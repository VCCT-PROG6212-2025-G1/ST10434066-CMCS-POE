using CMCS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Tests;
//helpers for unit tests. this helps create a new in-memory database context for each test to ensure isolation.
internal static class TestHelpers
{
    public static AppDbContext NewInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

        return new AppDbContext(options);
    }
}