using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public RoleEnum Role { get; set; }
        public ICollection<LeaveRequest> SubmittedRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<LeaveRequest> ReviewedRequests { get; set; } = new List<LeaveRequest>();
    }
}
