using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Services
{
    public class AdminService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;

        public AdminService(
            ILeaveRequestRepository leaveRequestRepo
            )
        {
            _leaveRequestRepo = leaveRequestRepo;
        }

        public List<LeaveRequest> GetAllRequests()
        {
            return _leaveRequestRepo.GetAll().OrderBy(r => (int)r.Status).ToList();
        }
    }
}
