using FluentAssertions;
using LeaveTrackerSystem.Application.Services;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Mock;
using LeaveTrackerSystem.Infrastructure.Repositories;

namespace LeaveTrackerSystem.Testing.Services
{
    public class LeaveBalanceServiceTests
    {
        [Fact]
        public void GetRemainingLeave_ShouldReturnCorrectBalance_WhenApprovedLeaveExists()
        {
            // Arrange
            InMemoryData.LeaveRequests.Clear();

            InMemoryData.LeaveRequests.Add(new LeaveRequest
            {
                Email = "alice@example.com",
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 5),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Approved
            });

            var repo = new InMemoryLeaveRequestRepository();
            var service = new LeaveBalanceService(repo);

            // Act
            var result = service.GetRemainingLeave("alice@example.com", LeaveType.Annual);

            // Assert
            result.Should().Be(9); // 14 - 5 days
        }

        [Fact]
        public void GetRemainingLeave_ShouldReturnZero_WhenUsedEqualsEntitlement()
        {
            // Arrange
            InMemoryData.LeaveRequests.Clear();

            InMemoryData.LeaveRequests.Add(new LeaveRequest
            {
                Email = "alice@example.com",
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 14),
                LeaveType = LeaveType.Annual,
                Status = LeaveStatus.Approved
            });

            var repo = new InMemoryLeaveRequestRepository();
            var service = new LeaveBalanceService(repo);

            // Act
            var result = service.GetRemainingLeave("alice@example.com", LeaveType.Annual);

            // Assert
            result.Should().Be(0); // 14 - 14 = 0
        }

        [Fact]
        public void GetRemainingLeave_ShouldCalculateCorrectly_ForCrossYearLeave()
        {
            // Arrange
            InMemoryData.LeaveRequests.Clear();

            InMemoryData.LeaveRequests.Add(new LeaveRequest
            {
                Email = "alice@example.com",
                StartDate = new DateTime(2025, 12, 30),
                EndDate = new DateTime(2026, 1, 2),
                LeaveType = LeaveType.Medical,
                Status = LeaveStatus.Approved
            });

            var repo = new InMemoryLeaveRequestRepository();
            var service = new LeaveBalanceService(repo);

            // Act
            var result = service.GetRemainingLeave("alice@example.com", LeaveType.Medical);

            // Assert
            result.Should().Be(6); // 10 - 4 = 6
        }
    }
}
