using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface IAdminService
    {
        List<LeaveRequest> GetAllRequests();
    }
}
