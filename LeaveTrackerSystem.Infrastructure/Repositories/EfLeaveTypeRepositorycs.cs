using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Infrastructure.Persistence;

namespace LeaveTrackerSystem.Infrastructure.Repositories
{
    public class EfLeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly LeaveTrackerDbContext _dbContext;

        public EfLeaveTypeRepository(LeaveTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<LeaveType> GetAll() => _dbContext.LeaveTypes.ToList();

        public LeaveType? GetById(int id) => _dbContext.LeaveTypes.FirstOrDefault(t => t.Id == id);
    }
}
