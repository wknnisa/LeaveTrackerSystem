using LeaveTrackerSystem.Application.DTOs;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface IEmployeeService
    {
        List<LeaveTypeDto> GetLeaveTypes();
        (bool success, string message) SubmitLeaveRequest(string email, LeaveRequestDto dto);
        List<LeaveRequest> GetMyRequests(string email, string? status);
        Dictionary<string, (int Used, int Remaining)> GetLeaveSummary(string email, LeaveStatus? statusFilter = null);
        Dictionary<string, int> GetMonthlyUsage(string email);
    }
}
