namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class EOQControllerTests
    {
        private ISessionService sessionService;
        private IEOQService eoqService;

        [Fact]
        public async Task GetYears_WhenCalled_ReturnAsExpectedYears()
        {
            // Arrange
            IEnumerable<int> years = new List<int> { 2018, 2019, 2020 };
            var controller = GetEOQController();
            eoqService.GetYearsAsync().Returns(years);

            // Act
            var actual = await controller.GetYears();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<int>>(jsonResult.Value);
            Assert.Equal(years, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private EOQController GetEOQController()
        {
            sessionService = FakeSessionService();
            eoqService = Substitute.For<IEOQService>();
            var controller = new EOQController(sessionService, eoqService);

            return controller;
        }

        #endregion Helpers
    }
}