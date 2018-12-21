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

        [Fact]
        public async Task GetPerformanceAppraisalResult_WhenCalled_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var expected = new List<PARecordModel>
            {
                new PARecordModel { PARecordId = 1, Employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" } }
            };
            var homeController = GetHomeController();
            employeeService.GetPerformanceAppraisalResultAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await homeController.GetPerformanceAppraisalResult();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PARecordModel>>(jsonResult.Value);
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
            employeeService.GetNominationResultAsync(sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await homeController.GetNominationResult();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<EmployeeNominationModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
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