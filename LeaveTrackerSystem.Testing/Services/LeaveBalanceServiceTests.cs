using FluentAssertions;
using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using LeaveTrackerSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeaveTrackerSystem.Testing.Services
{
    public class LeaveBalanceServiceTests
    {
        private LeaveTrackerDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<LeaveTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new LeaveTrackerDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        [Fact]
        public void GetRemainingLeave_ShouldReturnCorrectBalance_WhenApprovedLeaveExists()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext("TestDb1");

            var user = new User { Email = "alice1@example.com", Name = "Alice" };
            var type = new LeaveType { Name = "Annual"};

            dbContext.Users.Add(user);
            dbContext.LeaveTypes.Add(type);
            dbContext.SaveChanges();

            dbContext.LeaveRequests.Add(new LeaveRequest
            {
                UserId = user.Id,
                LeaveTypeId = type.Id,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 5),
                Status = LeaveStatus.Approved
            }); 

            dbContext.SaveChanges();

            var repo = new EfLeaveRequestRepository(dbContext);
            var service = new LeaveBalanceService(repo);

            // Act
            var result = service.GetRemainingLeave("alice1@example.com", LeaveTypeEnum.Annual);

            // Assert
            result.Should().Be(9); // 14 - 5 days
        }

        [Fact]
        public void GetRemainingLeave_ShouldReturnZero_WhenUsedEqualsEntitlement()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext("TestDb2");

            var user = new User { Email = "alice2@example.com", Name = "Alice" };
            var type = new LeaveType { Name = "Annual" };

            dbContext.Users.Add(user);
            dbContext.LeaveTypes.Add(type);
            dbContext.SaveChanges();

            dbContext.LeaveRequests.Add(new LeaveRequest
            {
                UserId = user.Id,
                LeaveTypeId = type.Id,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 14),
                Status = LeaveStatus.Approved
            });

            dbContext.SaveChanges();

            var repo = new EfLeaveRequestRepository(dbContext);
            var service = new LeaveBalanceService(repo);

            // Act
            var result = service.GetRemainingLeave("alice2@example.com", LeaveTypeEnum.Annual);

            // Assert
            result.Should().Be(0); // 14 - 14 = 0
        }

        [Fact]
        public void GetRemainingLeave_ShouldCalculateCorrectly_ForCrossYearLeave()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext("TestDb3");

            var user = new User { Email = "alice3@example.com", Name = "Alice" };
            var type = new LeaveType { Name = "Medical" };

            dbContext.Users.Add(user);
            dbContext.LeaveTypes.Add(type);
            dbContext.SaveChanges();

            dbContext.LeaveRequests.Add(new LeaveRequest
            {
                UserId = user.Id,
                LeaveTypeId = type.Id,
                StartDate = new DateTime(2025, 12, 30),
                EndDate = new DateTime(2026, 1, 2),
                Status = LeaveStatus.Approved
            });

            dbContext.SaveChanges();

            var repo = new EfLeaveRequestRepository(dbContext);
            var service = new LeaveBalanceService(repo);

            // Act
            var result = service.GetRemainingLeave("alice3@example.com", LeaveTypeEnum.Medical);

            // Assert
            result.Should().Be(6); // 10 - 4 = 6
        }
    }
}