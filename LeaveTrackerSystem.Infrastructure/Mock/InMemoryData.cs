using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Infrastructure.Mock
{
    public static class InMemoryData
    {
        public static List <LeaveRequest> LeaveRequests { get; } = new();
    }
}
