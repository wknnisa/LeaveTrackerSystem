using System.ComponentModel.DataAnnotations;

namespace LeaveTrackerSystem.Domain.Enums
{
    public enum LeaveTypeEnum
    {
        Unknown = 0,
        [Display(Name = "Annual Leave")]
        Annual = 1,
        [Display(Name = "Medical Leave")]
        Medical = 2,
        [Display(Name = "Emergency Leave")]
        Emergency = 3
    }
}
