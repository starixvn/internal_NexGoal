namespace NexGoal.Controllers.UnitTests
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class HomeControllerTests
    {
        private IDashboardService dashboardService;

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

        private HomeController GetHomeController()
        {
            dashboardService = Substitute.For<IDashboardService>();
            var controller = new HomeController(dashboardService);

            return controller;
        }

        #endregion Helpers
    }
}