using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Domain.Enums;
using LeaveTrackerSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveTrackerSystem.Infrastructure.Repositories
{
    public class EfLeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly LeaveTrackerDbContext _dbContext;

        public EfLeaveRequestRepository(LeaveTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<LeaveRequest> GetApprovedRequestsByEmailAndType(string email, LeaveTypeEnum type)
        {
            var typeName = type.ToString();

            return _dbContext.LeaveRequests
                .Include(r => r.User)
                .Include(r => r.LeaveType)
                .Where(r => r.User.Email == email && r.LeaveType.Name == typeName && r.Status == LeaveStatus.Approved)
                .ToList();
        }
    }
}
