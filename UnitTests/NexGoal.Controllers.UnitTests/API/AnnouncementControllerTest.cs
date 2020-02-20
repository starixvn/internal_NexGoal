namespace NexGoal.Controllers.UnitTests.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Controllers.API;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class AnnouncementControllerTest
    {
        private ISessionService sessionService;
        private ICommonService commonService;

        [Fact]
        public async Task GetAnnouncements_WhenCalled_ReturnAsExpectedAnnouncements()
        {
            // Arrange
            IEnumerable<string> announcements = new List<string> { "abc", "def" };
            var controller = GetAnnouncementController();
            const int employeeId = 1;
            commonService.GetAnnouncementAsync(employeeId).Returns(announcements);

            // Act
            var actual = await controller.GetAnnouncements(employeeId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<string>>(jsonResult.Value);
            Assert.Equal(announcements, result);
        }

        #region Helpers

        private AnnouncementController GetAnnouncementController()
        {
            sessionService = Helpers.FakeSessionService();
            commonService = Substitute.For<ICommonService>();
            return new AnnouncementController(sessionService, commonService);
        }

        #endregion Helpers
    }
}