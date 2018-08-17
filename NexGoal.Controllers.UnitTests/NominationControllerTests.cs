namespace NexGoal.Controllers.UnitTests
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.ViewModels.EOQY;
    using NSubstitute;
    using Xunit;

    public class NominationControllerTests
    {
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var activeTab = NominationController.NavTabs.EOQ;
            var controller = GetNominationController();
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = controller.Index(activeTab);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<NominationManagementViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal((int)activeTab, viewModel.ActiveTab);
            Assert.True(viewModel.IsHost);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private NominationController GetNominationController()
        {
            sessionService = FakeSessionService();
            var controller = new NominationController(sessionService);

            return controller;
        }

        #endregion Helpers
    }
}