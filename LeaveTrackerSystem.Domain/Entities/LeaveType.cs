namespace LeaveTrackerSystem.Domain.Entities
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int DefaultDays { get; set; }
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
