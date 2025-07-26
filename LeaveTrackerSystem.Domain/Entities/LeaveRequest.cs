using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    }
}
