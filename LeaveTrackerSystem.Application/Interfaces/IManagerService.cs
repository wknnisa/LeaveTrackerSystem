using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface IManagerService
    {
        List<LeaveRequest> GetAllRequestsForManager(string email, string? status);
        public LeaveRequest? UpdateLeaveStatus(int requestId, LeaveStatus newStatus);
    }
}
