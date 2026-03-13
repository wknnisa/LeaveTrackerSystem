using LeaveTrackerSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveTypesController : ControllerBase
    {
        private readonly LeaveTrackerDbContext _dbContext;

        public LeaveTypesController(LeaveTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetLeaveTypes()
        {
            var leaveTypes = _dbContext.LeaveTypes.Select(x => new
            {
                x.Id,
                x.Name
            });

            return Ok(leaveTypes);
        }
    }
}
