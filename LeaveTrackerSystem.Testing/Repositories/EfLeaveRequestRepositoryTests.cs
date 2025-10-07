using FluentAssertions;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using LeaveTrackerSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeaveTrackerSystem.Testing.Repositories
{
    public class EfLeaveRequestRepositoryTests
    {
        private LeaveTrackerDbContext _dbContext;
        private readonly EfLeaveRequestRepository _repository;

        public EfLeaveRequestRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LeaveTrackerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new LeaveTrackerDbContext(options);
            _repository = new EfLeaveRequestRepository(_dbContext);
        }

        [Fact]
        public void GetRequestByStatus_ShouldReturnFilteredResults()
        {
            // Arrange
            var user = new User { Email = "test@company.com", Name = "Tester" };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var approved = new LeaveRequest
            {
                UserId = user.Id,
                LeaveType = new LeaveType { Name = "Annual", DefaultDays = 14 },
                Status = LeaveStatus.Approved
            };
            var pending = new LeaveRequest()
            {
                UserId = user.Id,
                LeaveType = new LeaveType { Name = "Sick", DefaultDays = 10 },
                Status = LeaveStatus.Pending
            };
            _dbContext.LeaveRequests.AddRange(approved, pending);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.GetRequestByStatus("test@company.com", LeaveStatus.Approved);

            // Assert
            result.Should().HaveCount(1);
            result.First().Status.Should().Be(LeaveStatus.Approved);
        }

        [Fact]
        public void Add_ShouldInsertRecordIntoDatabase()
        {
            var user = new User { Email = "add@example.com" };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var newRequest = new LeaveRequest()
            {
                User = user, 
                Status = LeaveStatus.Pending,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            };

            _repository.Add(newRequest);
            _dbContext.SaveChanges();

            _dbContext.LeaveRequests.Should().ContainSingle();
        }
    }
}
