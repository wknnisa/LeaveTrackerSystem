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

        private readonly Dictionary<LeaveTypeEnum, int> _entitlement = new()
        {
            { LeaveTypeEnum.Annual, 14 },
            { LeaveTypeEnum.Medical, 10 },
            { LeaveTypeEnum.Emergency, 5 }
        };

        public int GetUsedLeaveDays(string email, LeaveTypeEnum type)
        {
            var approvedRequests = _repo.GetApprovedRequestsByEmailAndType(email, type);
            
            return approvedRequests.Sum(r => (r.EndDate - r.StartDate).Days + 1);
        }

        public int GetRemainingLeave(string email, LeaveTypeEnum type) 
        {
            var used = GetUsedLeaveDays(email, type);

            return _entitlement.TryGetValue(type, out var total) ? Math.Max(0, total-used) : 0;
        }

        public Dictionary<string, (int used, int remaining)> GetLeaveSummary(string email, LeaveStatus? statusFilter = null)
        {
            var summary = new Dictionary<string, (int used, int remaining)>();

            foreach (LeaveTypeEnum type in Enum.GetValues(typeof(LeaveTypeEnum)))
            {
                int used = 0;

                if (statusFilter == null || statusFilter == LeaveStatus.Approved)
                {
                    used = GetUsedLeaveDays(email, type);
                }

                int remaining = _entitlement.TryGetValue(type, out var total) ? Math.Max(0, total - used) : 0;

                summary[type.ToString()] = (used, remaining);
            }

            return summary;
        }
    }
}
