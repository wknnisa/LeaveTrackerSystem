using LeaveTrackerSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTrackerSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public LeaveRequestsController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("{email}")]
        public IActionResult GetRequests(string email)
        {
            var result = _employeeService.GetMyRequests(email, null, 1, 50);

            var response = result.Requests.Select(r => new
            {
                r.Id,
                r.StartDate,
                r.EndDate,
                r.Reason,
                Status = r.Status.ToString(),
                LeaveType = r.LeaveType?.Name
           });

            return Ok(response);
        }
    }
}
