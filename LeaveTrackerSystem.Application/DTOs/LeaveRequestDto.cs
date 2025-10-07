namespace LeaveTrackerSystem.Application.DTOs
{
    public class LeaveRequestDto
    {
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
