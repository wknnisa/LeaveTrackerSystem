using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface INotificationService
    {
        void NotifyLeaveSubmission(string userEmail, LeaveRequest request);
        void NotifyLeaveApproval(string managerEmail, LeaveRequest request, bool approved);
    }
}
