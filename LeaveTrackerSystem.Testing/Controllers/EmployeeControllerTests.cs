using LeaveTrackerSystem.Application.Interfaces;
using LeaveTrackerSystem.WebApp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LeaveTrackerSystem.Testing.Controllers
{
    public class EmployeeControllerTests
    {
        [Fact]
        public void MyRequests_ShouldRedirectToLogin_WhenSessionExpired()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();
            var controller = new EmployeeController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = controller.MyRequests(null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }
    }
}
