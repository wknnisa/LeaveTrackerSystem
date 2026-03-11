using FluentAssertions;
using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using LeaveTrackerSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeaveTrackerSystem.Testing.Services
{
    public class ManagerServiceTests
    {
        private readonly LeaveTrackerDbContext _dbContext;
        private readonly ManagerService _service;

        public ManagerServiceTests()
        {
            var options = new DbContextOptionsBuilder<LeaveTrackerDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            _dbContext = new LeaveTrackerDbContext(options);

            var leaveRepo = new EfLeaveRequestRepository(_dbContext);
            var userRepo = new EfUserRepository(_dbContext);

            _service = new ManagerService(leaveRepo, userRepo);
        }

        [Fact]
        public void UpdateLeaveStatus_ShouldApprovePendingRequest()
        {
            // Arrange
            var request = new LeaveRequest
            {
                Status = LeaveStatus.Pending,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            };

            _dbContext.LeaveRequests.Add(request);
            _dbContext.SaveChanges();

            // Act
            var result = _service.UpdateLeaveStatus(request.Id, LeaveStatus.Approved);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(LeaveStatus.Approved);
        }

        [Fact]
        public void UpdateLeaveStatus_ShouldRejectPendingRequest()
        {
            // Arrange
            var request = new LeaveRequest
            {
                Status = LeaveStatus.Pending,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            };

            
            _dbContext.LeaveRequests.Add(request);
            _dbContext.SaveChanges();

            // Act
            var result = _service.UpdateLeaveStatus(request.Id, LeaveStatus.Rejected);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(LeaveStatus.Rejected);
        }

        [Fact]
        public void UpdateLeaveStatus_ShouldReturnNull_WhenRequestAlreadyProcessed()
        {
            // Arrange
            var request = new LeaveRequest
            {
                Status = LeaveStatus.Rejected,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            };

            _dbContext.LeaveRequests.Add(request);
            _dbContext.SaveChanges();

            // Act
            var result = _service.UpdateLeaveStatus(request.Id, LeaveStatus.Approved);

            // Assert
            result.Should().BeNull();
        }
    }
}
