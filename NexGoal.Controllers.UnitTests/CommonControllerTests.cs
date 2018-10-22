namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class CommonControllerTests
    {
        private ICommonService commonService;

        [Fact]
        public async Task SearchEmployees_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            var controller = GetCommonController();
            commonService.GetEmployeesAsync().Returns(employees);

            // Act
            var actual = await controller.SearchEmployees();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);
            Assert.Equal(employees, result);
        }

        [Fact]
        public async Task GetGroups_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var groups = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var controller = GetCommonController();
            commonService.GetGroupsAsync().Returns(groups);

            // Act
            var actual = await controller.GetGroups();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<GroupBaseModel>>(jsonResult.Value);
            Assert.Equal(groups, result);
        }

        [Fact]
        public async Task GetTargetTypes_WhenCalled_ReturnAsExpectedTargetTypes()
        {
            // Arrange
            var referenceResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "Semi-annual" }
            };
            var controller = GetCommonController();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(referenceResources);

            // Act
            var actual = await controller.GetTargetTypes();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<TargetTypeModel>>(jsonResult.Value);
            Assert.Equal(referenceResources.Count, result.Count());
        }

        [Fact]
        public async Task GetEvaluateStatuses_WhenCalled_ReturnAsExpectedEvaluateStatuses()
        {
            // Arrange
            var referenceResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "Evaluated" }
            };
            var controller = GetCommonController();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(referenceResources);

            // Act
            var actual = await controller.GetEvaluateStatuses();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<EvaluateStatusModel>>(jsonResult.Value);
            Assert.Equal(referenceResources.Count, result.Count());
        }

        [Fact]
        public async Task GetPerformanceStatuses_WhenCalled_ReturnAsExpectedPerformanceStatuses()
        {
            // Arrange
            var referenceResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "Open" }
            };
            var controller = GetCommonController();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(referenceResources);

            // Act
            var actual = await controller.GetPerformanceStatuses();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<PerformanceStatusModel>>(jsonResult.Value);
            Assert.Equal(referenceResources.Count, result.Count());
        }

        [Fact]
        public async Task GetProjects_WhenCalled_ReturnAsExpectedProjects()
        {
            // Arrange
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "NexGoal" }
            };
            var controller = GetCommonController();
            commonService.GetProjectsAsync().Returns(projects);

            // Act
            var actual = await controller.GetProjects();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<ProjectModel>>(jsonResult.Value);
            Assert.Equal(projects, result);
        }

        [Fact]
        public async Task GetPerformanceGrades_WhenCalled_ReturnAsExpectedPerformanceGrades()
        {
            // Arrange
            var performanceGrades = new List<PerformanceGradeModel>
            {
                new PerformanceGradeModel { GradeId = 1, Name = "Unsatisfactory" }
            };
            var controller = GetCommonController();
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);

            // Act
            var actual = await controller.GetPerformanceGrades();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<PerformanceGradeModel>>(jsonResult.Value);
            Assert.Equal(performanceGrades, result);
        }

        [Fact]
        public async Task GetAllPeriods_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "1h-2018" }
            };
            var controller = GetCommonController();
            commonService.GetAllPeriods().Returns(periods);

            // Act
            var actual = await controller.GetAllPeriods();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<PeriodBaseModel>>(jsonResult.Value);
            Assert.Equal(periods, result);
        }

        #region Helpers

        private CommonController GetCommonController()
        {
            commonService = Substitute.For<ICommonService>();
            var controller = new CommonController(commonService);

            return controller;
        }

        #endregion Helpers
    }
}