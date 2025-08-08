using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType{ get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = "";
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.Now;
        public int? ReviewedByUserId  { get; set; }
        public User? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Comments { get; set; }
    }
}
