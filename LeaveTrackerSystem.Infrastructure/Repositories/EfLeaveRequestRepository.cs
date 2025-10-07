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

        public void Add(LeaveRequest leaveRequest)
        {
            _dbContext.LeaveRequests.Add(leaveRequest);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public List<LeaveRequest> GetByUserEmail(string email)
        {
            return _dbContext.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.User)
                .Where(r => r.User.Email == email)
                .ToList();
        }

        public List<LeaveRequest> GetByUserId(int userId)
        {
            return _dbContext.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .ToList();
        }

        public List<LeaveRequest> GetAll()
        {
            return _dbContext.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.User)
                .ToList();
        }

        public LeaveRequest? GetById(int id) => _dbContext.LeaveRequests.FirstOrDefault(r => r.Id == id);

        public List<LeaveRequest> GetRequestByStatus(string email, LeaveStatus? status)
        {
            var query = _dbContext.LeaveRequests
                .Include(r => r.User)
                .Include(r => r.LeaveType)
                .Where(r => r.User.Email == email);

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return query.ToList();
        }
    }
}