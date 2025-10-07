using LeaveTrackerSystem.Application.DTOs;
using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;

namespace LeaveTrackerSystem.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly IUserRepository _userRepo;

        public EmployeeService(
            ILeaveRequestRepository leaveRequestRepo,
            ILeaveTypeRepository leaveTypeRepository,
            IUserRepository userRepository
            )
        {
            _leaveRequestRepo = leaveRequestRepo;
            _leaveTypeRepo = leaveTypeRepository;
            _userRepo = userRepository;
        }

        public List<LeaveTypeDto> GetLeaveTypes()
        {
            return _leaveTypeRepo.GetAll().Select(t => new LeaveTypeDto { Id = t.Id, Name = t.Name }).ToList();
        }

        public (bool success, string message) SubmitLeaveRequest(string email, LeaveRequestDto dto)
        {
            var user = _userRepo.GetByEmail(email);

            if (user == null)
            {
                return (false, "User not found.");
            }

            var leaveType = _leaveTypeRepo.GetById(dto.LeaveTypeId);

            if (leaveType == null)
            {
                return (false, "Leave type not found.");
            }

            var daysRequested = (dto.EndDate - dto.StartDate).TotalDays + 1;

            if (daysRequested > leaveType.DefaultDays)
            {
                return (false, "Requested days exceed your leave entitlement.");
            }

            var request = new LeaveRequest
            {
                UserId = user.Id,
                LeaveTypeId = leaveType.Id,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = LeaveStatus.Pending,
                RequestedAt = DateTime.Now,
            };

            _leaveRequestRepo.Add(request);
            _leaveRequestRepo.SaveChanges();

            return (true, "Leave request submitted successfully.");
        }

        public List<LeaveRequest> GetMyRequests(string email, string? status)
        {
            var requests = _leaveRequestRepo.GetByUserEmail(email);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var parsed))
            {
                requests = requests.Where(r => r.Status == parsed).ToList();
            }

            return requests.OrderBy(r => (int)r.Status).ToList();
        }

        public Dictionary<string, (int Used, int Remaining)> GetLeaveSummary(string email, LeaveStatus? statusFilter = null)
        {
            var user = _userRepo.GetByEmail(email);

            if (user == null)
            {
                return new();
            }

            var leaveTypes = _leaveTypeRepo.GetAll();
            var requests = _leaveRequestRepo.GetRequestByStatus(email, statusFilter);

            var summary = leaveTypes.ToDictionary(lt => lt.Name,
                lt =>
                {
                    var approved = requests.Where(r => r.LeaveTypeId == lt.Id && r.Status == LeaveStatus.Approved).ToList();
                    var usedDays = approved.Sum(r => (r.EndDate - r.StartDate).TotalDays + 1);
                    var remaining = Math.Max(0, lt.DefaultDays - (int)usedDays);

                    return ((int)usedDays, remaining);
                });

            return summary;
        }

        public Dictionary<string, int> GetMonthlyUsage(string email)
        {
            var user = _userRepo.GetByEmail(email);

            if (user == null)
            {
                return new();
            }

            var requests = _leaveRequestRepo.GetByUserEmail(email)
                .Where(r => r.Status == LeaveStatus.Approved)
                .GroupBy(r => new DateTime(r.StartDate.Year, r.StartDate.Month, 1))
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => g.Key.ToString("MM/yyyy"),
                    g => g.Count()
                );

            return requests;
        }
    }
}
