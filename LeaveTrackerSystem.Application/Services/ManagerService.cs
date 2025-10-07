using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Services
{
    public class ManagerService
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
                return new List<LeaveRequest>();
            }

            var requests = _leaveRequestRepo.GetAll().Where(r => r.UserId != currentUser.Id);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                requests = requests.Where(r => r.Status == parsed);
            }

            return requests.OrderBy(r => (int)r.Status).ToList();
        }

        public bool UpdateLeaveStatus(int requestId, LeaveStatus newStatus)
        {
            var request = _leaveRequestRepo.GetById(requestId);

            if (request == null || request.Status != LeaveStatus.Pending)
            {
                return false;
            }

            request.Status = newStatus;
            _leaveRequestRepo.SaveChanges();
            return true;
        }
    }
}
