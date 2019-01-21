namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
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
            var controller = GetContronller();

            var actual = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetSetting_WhenCalled_ReturnsExpectedJsonResult()
        {
            var controller = GetContronller();
            nominationReportService.GetNominationPeriodsAsync().Returns(new List<NominationPeriodModel>
            {
                new NominationPeriodModel
                {
                    NominationPeriodId =1,
                    Year = 2018,
                }
            });
            var actual = await controller.GetNominationPeriods();

            var jsonResult = Assert.IsType<JsonResult>(actual);
            var value = Assert.IsAssignableFrom<IEnumerable<NominationPeriodModel>>(jsonResult.Value);

            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task GetNominations_WhenCalled_ReturnsExpectedJsonResult()
        {
            var controller = GetContronller();
            nominationReportService.GetNominationsAsync("1", null, null, null, sessionService.EmployeeId).Returns(new List<NominationModel>
            {
                new NominationModel
                {
                    NominationId = 1,
                    NominationPeriodId = 1,
                    NomineeId = 2
                }
            });
            var actual = await controller.GetNominations("1", null, null, null);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            var value = Assert.IsAssignableFrom<IEnumerable<NominationModel>>(jsonResult.Value);

            Assert.NotEmpty(value);
        }

        private NominationReportController GetContronller()
        {
            nominationReportService = Substitute.For<INominationReportService>();
            sessionService = Helpers.FakeSessionService();

            var controller = new NominationReportController(nominationReportService, sessionService);

            return controller;
        }
    }
}