using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface IManagerService
    {
        List<LeaveRequest> GetAllRequestsForManager(string email, string? status);
        (List<LeaveRequest> Requests, bool HasNextPage) GetAllRequestsForManager(string email, string? status, int page, int pageSize);
        public LeaveRequest? UpdateLeaveStatus(int requestId, LeaveStatus newStatus);
    }
}
