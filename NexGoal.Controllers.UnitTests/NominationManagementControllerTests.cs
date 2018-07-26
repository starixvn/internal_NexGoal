namespace NexGoal.Controllers.UnitTests
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.ViewModels.EOQY;
    using NSubstitute;
    using Xunit;

    public class NominationManagementControllerTests
    {
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var activeTab = NominationManagementController.NavTabs.EOQ;
            var controller = GetNominationManagementController();
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

        private NominationManagementController GetNominationManagementController()
        {
            sessionService = FakeSessionService();
            var controller = new NominationManagementController(sessionService);

            return controller;
        }

        #endregion Helpers
    }
}