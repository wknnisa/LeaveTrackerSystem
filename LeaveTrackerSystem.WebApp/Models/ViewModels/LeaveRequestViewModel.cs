using LeaveTrackerSystem.WebApp.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LeaveTrackerSystem.WebApp.Models.ViewModels
{
    public class LeaveRequestViewModel
    {
        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        [DateGreaterThan("StartDate", ErrorMessage = "End Date must be on or after Start Date.")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Leave Type is required.")]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        public List<SelectListItem> LeaveTypes { get; set; } = new();

        [Required(ErrorMessage = "Reason is required.")]
        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;
    }
}
