namespace NexGoal.Controllers.UnitTests
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class HomeControllerTests
    {
        private ISessionService sessionService;
        private IEmployeeService employeeService;
        private IRedmineService redmineService;

        [Fact]
        public void Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var homeController = GetHomeController();

            // Act
            var actual = homeController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private HomeController GetHomeController()
        {
            sessionService = FakeSessionService();
            employeeService = Substitute.For<IEmployeeService>();
            redmineService = Substitute.For<IRedmineService>();
            var controller = new HomeController(sessionService, employeeService, redmineService);

            return controller;
        }

        #endregion Helpers
    }
}