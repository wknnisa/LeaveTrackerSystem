using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using LeaveTrackerSystem.Infrastructure.Persistence;

namespace LeaveTrackerSystem.Infrastructure.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly LeaveTrackerDbContext _dbContext;

        public EfUserRepository(LeaveTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User? GetByEmail(string email) => _dbContext.Users.FirstOrDefault(u => u.Email == email);
    }
}
