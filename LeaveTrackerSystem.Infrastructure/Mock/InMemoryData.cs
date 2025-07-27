using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Infrastructure.Mock
{
    public static class InMemoryData
    {
        public static List <LeaveRequest> LeaveRequests { get; } = new();

        public static class LeaveBalanceStore
        {
            public static Dictionary<string, Dictionary<LeaveType, int>> UsedLeave = new();

            public static Dictionary<LeaveType, int> MaxBalance = new()
            {
                {LeaveType.Annual, 14},
                {LeaveType.Medical, 10},
                {LeaveType.Emergency, 5},
            };
        }
    }
}
