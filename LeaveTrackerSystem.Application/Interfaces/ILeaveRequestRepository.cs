using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface ILeaveRequestRepository
    {
        List<LeaveRequest> GetApprovedRequestsByEmailAndType(string email, LeaveTypeEnum type);
    }
}
