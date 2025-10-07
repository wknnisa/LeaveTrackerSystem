using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);
    }
}
