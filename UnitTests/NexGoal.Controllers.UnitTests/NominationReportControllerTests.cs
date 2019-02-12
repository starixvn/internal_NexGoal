namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class NominationReportControllerTests
    {
        private ISessionService sessionService;
        private INominationReportService nominationReportService;

        [Fact]
        public void Index_WhenCalled_ReturnsExpectedViewResult()
        {
            // Arrange
            var controller = GetContronller();

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetSetting_WhenCalled_ReturnsExpectedJsonResult()
        {
            // Arrange
            var nominationPeriods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel
                {
                    NominationPeriodId = 1,
                    Year = 2019
                }
            };
            var controller = GetContronller();
            nominationReportService.GetNominationPeriodAsync().Returns(nominationPeriods);

            // Act
            var result = await controller.GetNominationPeriods();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<NominationPeriodModel>>(jsonResult.Value);
            Assert.Equal(nominationPeriods, actual);
        }

        [Fact]
        public async Task GetNominations_WhenCalled_ReturnsExpectedJsonResult()
        {
            // Arrange
            var nominations = new List<NominationModel>
            {
                new NominationModel
                {
                    NominationId = 1,
                    Period = new NominationPeriodModel { NominationPeriodId = 1, Year = 2019 },
                    Nominee = new Employee { EmployeeId = 1, NickName = "tim" }
                }
            };
            var controller = GetContronller();
            nominationReportService.GetNominationAsync(sessionService.EmployeeId, "1", null, null, null).Returns(nominations);

            // Act
            var result = await controller.GetNominations("1", null, null, null);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<NominationModel>>(jsonResult.Value);
            Assert.Equal(nominations, actual);
        }

        private NominationReportController GetContronller()
        {
            sessionService = Helpers.FakeSessionService();
            nominationReportService = Substitute.For<INominationReportService>();

            var controller = new NominationReportController(sessionService, nominationReportService);

            return controller;
        }
    }
}