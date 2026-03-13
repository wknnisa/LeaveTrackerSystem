using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly IUserRepository _userRepo;

        public ManagerService(
            ILeaveRequestRepository leaveRequestRepo,
            IUserRepository userRepository
            )
        {
            _leaveRequestRepo = leaveRequestRepo;
            _userRepo = userRepository;
        }

        public List<LeaveRequest> GetAllRequestsForManager(string email, string? status)
        {
            var currentUser = _userRepo.GetByEmail(email);

            if (currentUser == null)
            {
                return new();
            }

            var requests = _leaveRequestRepo.GetAll().Where(r => r.UserId != currentUser.Id);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                requests = requests.Where(r => r.Status == parsed);
            }

            return requests.OrderBy(r => (int)r.Status).ToList();
        }

        public (List<LeaveRequest> Requests, bool HasNextPage) GetAllRequestsForManager(string email, string? status, int page, int pageSize)
        {
            var currentUser = _userRepo.GetByEmail(email);

            if (currentUser == null)
            {
                return new();
            }

            var requests = _leaveRequestRepo.GetAll().Where(r => r.UserId != currentUser.Id);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
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

        public LeaveRequest? UpdateLeaveStatus(int requestId, LeaveStatus newStatus)
        {
            var request = _leaveRequestRepo.GetById(requestId);

            if (request is null || request.Status != LeaveStatus.Pending)
            {
                return null;
            }

            request.Status = newStatus;
            _leaveRequestRepo.SaveChanges();
            return request;
        }
    }
}
