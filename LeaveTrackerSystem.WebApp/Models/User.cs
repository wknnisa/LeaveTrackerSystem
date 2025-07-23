namespace LeaveTrackerSystem.WebApp.Models
{
    public class User
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = ""; // Admin, Manager, Employee
    }
}
