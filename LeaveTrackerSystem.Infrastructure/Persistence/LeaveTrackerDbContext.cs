using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveTrackerSystem.Infrastructure.Persistence
{
    public class LeaveTrackerDbContext : DbContext 
    {
        public LeaveTrackerDbContext(DbContextOptions<LeaveTrackerDbContext> options) : base(options)
        {
        }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { Id = 1, Name = "Annual", DefaultDays = 14 },
                new LeaveType { Id = 2, Name = "Medical", DefaultDays = 10 },
                new LeaveType { Id = 3, Name = "Emergency", DefaultDays = 5 }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Admin", Email = "admin@example.com", PasswordHash = "admin123", Role = RoleEnum.Admin },
                new User { Id = 2, Name = "Manager", Email = "manager@example.com", PasswordHash = "manager123", Role = RoleEnum.Manager },
                new User { Id = 3, Name = "Employee", Email = "employee@example.com", PasswordHash = "employee123", Role = RoleEnum.Employee }
            );

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.User)
                .WithMany(lr => lr.SubmittedRequests)
                .HasForeignKey(lr => lr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ReviewedBy)
                .WithMany(lr => lr.ReviewedRequests)
                .HasForeignKey(lr => lr.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
