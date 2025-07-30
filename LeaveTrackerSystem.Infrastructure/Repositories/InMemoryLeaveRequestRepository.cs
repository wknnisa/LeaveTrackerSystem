using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Mock;

namespace LeaveTrackerSystem.Infrastructure.Repositories
{
    public class InMemoryLeaveRequestRepository : ILeaveRequestRepository
    {
        public List<LeaveRequest> GetApprovedRequestsByEmailAndType(string email, LeaveType type)
        {
            return InMemoryData.LeaveRequests.Where(r => r.Email == email && r.LeaveType == type && r.Status == LeaveStatus.Approved)
                .ToList();
        }
    }
}
