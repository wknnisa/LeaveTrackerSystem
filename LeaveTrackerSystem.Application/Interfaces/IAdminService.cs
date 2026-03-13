using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface IAdminService
    {
        List<LeaveRequest> GetAllRequests();
        (List<LeaveRequest> Requests, bool HasNextPage) GetAllRequests(int page, int pageSize);
    }
}
