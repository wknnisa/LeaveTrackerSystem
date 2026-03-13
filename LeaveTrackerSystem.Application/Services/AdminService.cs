using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;

        public AdminService(ILeaveRequestRepository leaveRequestRepo)
        {
            _leaveRequestRepo = leaveRequestRepo;
        }

        public List<LeaveRequest> GetAllRequests()
        {
            return _leaveRequestRepo
                .GetAll()
                .OrderBy(r => (int)r.Status)
                .ToList();
        }

        public (List<LeaveRequest> Requests, bool HasNextPage) GetAllRequests(int page, int pageSize)
        {
            var requests = _leaveRequestRepo.GetAll().OrderBy(r => (int)r.Status);

            var paged = requests
                .Skip((page - 1) * pageSize)
                .Take(pageSize + 1)
                .ToList();

            var hasNextPage = paged.Count > pageSize;

            if (hasNextPage)
            {
                paged.RemoveAt(pageSize);
            }

            return (paged, hasNextPage);
                
        }
    }
}
