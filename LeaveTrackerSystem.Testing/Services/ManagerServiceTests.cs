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
            var request = new LeaveRequest
            {
                Status = LeaveStatus.Pending
            };

            _dbContext.LeaveRequests.Add(request);
            _dbContext.SaveChanges();

            var result = _service.UpdateLeaveStatus(request.Id, LeaveStatus.Approved);

            result.Should().NotBeNull();
            result.Status.Should().Be(LeaveStatus.Approved);
        }

        [Fact]
        public void UpdateLeaveStatus_ShouldRejectPendingRequest()
        {
            var request = new LeaveRequest
            {
                Status = LeaveStatus.Pending
            };

            _dbContext.LeaveRequests.Add(request);
            _dbContext.SaveChanges();

            var result = _service.UpdateLeaveStatus(request.Id, LeaveStatus.Rejected);

            result.Should().NotBeNull();
            result.Status.Should().Be(LeaveStatus.Rejected);
        }

        [Fact]
        public void UpdateLeaveStatus_ShouldReturnNull_WhenRequestAlreadyProcessed()
        {
            var request = new LeaveRequest
            {
                Status = LeaveStatus.Rejected
            };

            _dbContext.LeaveRequests.Add(request);
            _dbContext.SaveChanges();

            var result = _service.UpdateLeaveStatus(request.Id, LeaveStatus.Approved);

            result.Should().BeNull();
        }
    }
}
