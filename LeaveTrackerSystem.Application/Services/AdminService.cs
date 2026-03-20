using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

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

        public (List<LeaveRequest> Requests, bool HasNextPage) GetAllRequests(string? status, int page, int pageSize)
        {
            var requests = _leaveRequestRepo.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All" &&
                Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                requests = requests.Where(r => r.Status == parsed);
            }

            var paged = requests
                .OrderBy(r => (int)r.Status)
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
