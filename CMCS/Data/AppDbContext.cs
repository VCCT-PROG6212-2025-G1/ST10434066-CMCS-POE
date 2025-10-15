using CMCS.Models;
using Microsoft.EntityFrameworkCore;
// my local database to store the websites information
namespace CMCS.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Lecturer> Lecturers => Set<Lecturer>();
        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<SupportingDocument> SupportingDocuments => Set<SupportingDocument>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupportingDocument>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimID)
                .OnDelete(DeleteBehavior.Cascade); // Easier cascading structure 
        }
    }
    // Seeding Lecturers.
    public static class Seeding
    {
        public static void Ensure(AppDbContext db)
        {
            if (!db.Lecturers.Any())
            {
                db.Lecturers.AddRange(
                    new Lecturer { StaffNo = "L1", FirstName = "Lisa", LastName = "Smith", Email = "lisa@uni.co.za", HourlyRate = 500 },
                    new Lecturer { StaffNo = "L2", FirstName = "Fatima", LastName = "Zahra", Email = "fatima@uni.co.za", HourlyRate = 350 },
                    new Lecturer { StaffNo = "L3", FirstName = "Fay-yaad", LastName = "Parker", Email = "parker@uni.co.za", HourlyRate = 800 },
                    new Lecturer { StaffNo = "L4", FirstName = "Themba", LastName = "Molefe", Email = "themba@uni.co.za", HourlyRate = 200 }
                );
                db.SaveChanges();
            }
        }
    }
}