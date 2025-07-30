using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Services
{
    public class LeaveBalanceService
    {
        private readonly ILeaveRequestRepository _repo;

        public LeaveBalanceService(ILeaveRequestRepository repo)
        {
            _repo = repo;
        }

        private readonly Dictionary<LeaveType, int> _entitlement = new()
        {
            { LeaveType.Annual, 14 },
            { LeaveType.Medical, 10 },
            { LeaveType.Emergency, 7 }
        };

        public int GetUsedLeaveDays(string email, LeaveType type)
        {
            var approvedRequests = _repo.GetApprovedRequestsByEmailAndType(email, type);
            
            return approvedRequests.Sum(r => (r.EndDate - r.StartDate).Days + 1);
        }

        public int GetRemainingLeave(string email, LeaveType type) 
        {
            var used = GetUsedLeaveDays(email, type);

            return _entitlement.TryGetValue(type, out var total) ? Math.Max(0, total-used) : 0;
        }
    }
}
