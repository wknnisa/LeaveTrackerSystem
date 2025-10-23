using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace LeaveTrackerSystem.Infrastructure.Services
{
    public class NotificationService: INotificationService
    {
        private readonly bool _simulateEmails;

        public NotificationService(IConfiguration config)
        {
            _simulateEmails = config.GetValue<bool>("CustomSettings:SimulateEmails");
        }

        public void NotifyLeaveSubmission(string userEmail, LeaveRequest request) 
        { 
            if (!_simulateEmails)
            {
                return;
            }

            Debug.WriteLine($"📧 Email to Manager: New Leave Request from {userEmail} ({request.LeaveType?.Name})");
        }

        public void NotifyLeaveApproval(string managerEmail, LeaveRequest request, bool approved)
        {
            if (!_simulateEmails)
            {
                return;
            }

            Debug.WriteLine($"📧 Email to Employee: Your request from {request.StartDate:d}- {request.EndDate:d} was {(approved ? "approved" : "rejected")} by {managerEmail}");
        }
    }
}
