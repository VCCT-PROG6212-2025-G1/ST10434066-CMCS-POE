// Changed the entire DB class to add seeding functionality.
using CMCS.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Lecturer> Lecturers => Set<Lecturer>();
        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<SupportingDocument> SupportingDocuments => Set<SupportingDocument>();
        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupportingDocument>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAccount>()
                .HasOne(u => u.Lecturer)
                .WithMany()
                .HasForeignKey(u => u.LecturerID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public static class Seed
    {
        public static void Ensure(AppDbContext db)
        {
            if (!db.Lecturers.Any())
            {
                db.Lecturers.AddRange(
                    new Lecturer { StaffNo = "L001", FirstName = "Aisha", LastName = "Parker", Email = "aisha@vc.edu.za", HourlyRate = 500m },
                    new Lecturer { StaffNo = "L002", FirstName = "Fatima", LastName = "Waggie", Email = "fatima@vc.edu.za", HourlyRate = 450m }
                );
                db.SaveChanges();
            }
            // The m tells c# to treat the number as a decimal.

            if (!db.UserAccounts.Any())
            {
                var firstLecturer = db.Lecturers.First();

                db.UserAccounts.AddRange(
                    new UserAccount
                    {
                        Username = "HR",
                        Password = "HR123",
                        FullName = "HR Admin",
                        Role = "HR"
                    },
                    new UserAccount
                    {
                        Username = "Aisha",
                        Password = "AishaParker",
                        FullName = firstLecturer.FullName,
                        Role = "Lecturer",
                        LecturerID = firstLecturer.LecturerID
                    },
                    new UserAccount
                    {
                        Username = "coordinator",
                        Password = "coord",
                        FullName = "Programme Coordinator",
                        Role = "Coordinator"
                    },
                    new UserAccount
                    {
                        Username = "manager",
                        Password = "manager123",
                        FullName = "Academic Manager",
                        Role = "Manager"
                    }
                );

                db.SaveChanges();
            }
        }
    }
}