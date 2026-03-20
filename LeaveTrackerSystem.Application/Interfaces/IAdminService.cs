using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface IAdminService
    {
        List<LeaveRequest> GetAllRequests();
        (List<LeaveRequest> Requests, bool HasNextPage) GetAllRequests(string? status, int page, int pageSize);
    }
}
