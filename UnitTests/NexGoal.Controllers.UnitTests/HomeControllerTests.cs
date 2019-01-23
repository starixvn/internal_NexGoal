namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Redmine;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class HomeControllerTests
    {
        private ISessionService sessionService;
        private IEmployeeNominationService nominationService;
        private IPARecordService paRecordService;
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

        [Fact]
        public async Task GetPerformanceAppraisalResult_WhenCalled_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var expected = new List<PerformanceAppraisalResult>
            {
                new PerformanceAppraisalResult { PARecordId = 1, Employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" } }
            };
            var homeController = GetHomeController();
            paRecordService.GetPerformanceAppraisalResultAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await homeController.GetPerformanceAppraisalResult();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PerformanceAppraisalResult>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetWorkload_WhenCalled_ReturnAsExpectedWorkloads()
        {
            // Arrange
            var expected = new List<ProjectWorkloadModel>
            {
                new ProjectWorkloadModel { ProjectId = 1, Name = "Nexus", TotalHours = 100 }
            };
            var homeController = GetHomeController();
            redmineService.GetWorkloadAsync(sessionService.EmployeeId, 1).Returns(expected);

            // Act
            var result = await homeController.GetWorkload(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<ProjectWorkloadModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetNominationResult_WhenCalled_ReturnAsExpectedNominations()
        {
            // Arrange
            var expected = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel
                {
                    EmployeeId = 1, NickName = "Tim", GroupName = "Nexus",
                    Period = new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
                }
            };
            var homeController = GetHomeController();
            nominationService.GetNominationResultAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await homeController.GetNominationResult();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<EmployeeNominationModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private HomeController GetHomeController()
        {
            sessionService = Helpers.FakeSessionService();
            nominationService = Substitute.For<IEmployeeNominationService>();
            paRecordService = Substitute.For<IPARecordService>();
            redmineService = Substitute.For<IRedmineService>();
            var controller = new HomeController(sessionService, nominationService, paRecordService, redmineService);

            return controller;
        }

        #endregion Helpers
    }
}