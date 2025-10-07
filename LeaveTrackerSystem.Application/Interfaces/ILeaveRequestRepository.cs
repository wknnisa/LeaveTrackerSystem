using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface ILeaveRequestRepository
    {
        void Add(LeaveRequest leaveRequest);
        void SaveChanges();
        List<LeaveRequest> GetByUserEmail(string email);
        List<LeaveRequest> GetByUserId(int userId);
        List<LeaveRequest> GetAll();
        LeaveRequest? GetById(int id);
        List<LeaveRequest> GetRequestByStatus(string email, LeaveStatus? status);
    }
}
