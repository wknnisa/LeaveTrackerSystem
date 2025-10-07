using LeaveTrackerSystem.Domain.Entities;

namespace LeaveTrackerSystem.Application.Interfaces
{
    public interface ILeaveTypeRepository
    {
        List<LeaveType> GetAll();
        LeaveType? GetById(int id);
    }
}
