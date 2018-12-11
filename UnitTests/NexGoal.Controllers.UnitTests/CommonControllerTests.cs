namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class CommonControllerTests
    {
        private ISessionService sessionService;
        private ICommonService commonService;
        private IEmployeeService employeeService;

        [Fact]
        public async Task GetEmployees_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            var controller = GetCommonController();
            commonService.GetEmployeesAsync().Returns(employees);

            // Act
            var actual = await controller.GetEmployees();

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
        public async Task GetHRMSummary_WhenCalled_ReturnJson()
        {
            var controller = GetCommonController();
            var expectResult = new HRMSummaryModel
            {
                ToeicScore = 600,
                ToeicExamDate = DateTime.Now,
                Incident = 0
            };

            commonService.GetHRMSummary(363).Returns(expectResult);

            var result = await controller.GetHRMSummary();

            Assert.IsType<JsonResult>(result);
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

        [Fact]
        public async Task GetCurrentPerformancePeriod_WhenCalled_ReturnAsExpectedPeriod()
        {
            // Arrange
            var period = new PeriodBaseModel { PeriodId = 1, Name = "1h-2018" };
            var controller = GetCommonController();
            commonService.GetCurrentPerformancePeriodAsync().Returns(period);

            // Act
            var actual = await controller.GetCurrentPerformancePeriod();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<PeriodBaseModel>(jsonResult.Value);
            Assert.Equal(period, result);
        }

        [Fact]
        public async Task GetMyMember_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            var controller = GetCommonController();
            employeeService.GetMemberAsync(sessionService.EmployeeId).Returns(employees);

            // Act
            var actual = await controller.GetMyMember();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);
            Assert.Equal(employees, result);
        }

        [Fact]
        public async Task GetMyGroup_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var groups = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var controller = GetCommonController();
            employeeService.GetGroupAsync(sessionService.EmployeeId).Returns(groups);

            // Act
            var actual = await controller.GetMyGroup();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<GroupBaseModel>>(jsonResult.Value);
            Assert.Equal(groups, result);
        }

        [Fact]
        public async Task GetEOQQuestions_WhenCalled_ReturnAsExpectedQuestions()
        {
            // Arrange
            var referenceResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "How long have you been working with this nominee?" }
            };
            var controller = GetCommonController();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(referenceResources);

            // Act
            var actual = await controller.GetEOQQuestions();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<ReferenceResourceModel>>(jsonResult.Value);
            Assert.Equal(referenceResources, result);
        }

        [Fact]
        public async Task GetEOYQuestions_WhenCalled_ReturnAsExpectedQuestions()
        {
            // Arrange
            var referenceResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "How long have you been working with this nominee?" }
            };
            var controller = GetCommonController();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(referenceResources);

            // Act
            var actual = await controller.GetEOYQuestions();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<ReferenceResourceModel>>(jsonResult.Value);
            Assert.Equal(referenceResources, result);
        }

        [Fact]
        public async Task GetSystemRoles_WhenCalled_ReturnAsExpectedRoles()
        {
            // Arrange
            var referenceResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "SA" },
                new ReferenceResourceModel { ResourceId = 2, Value = "HR" }
            };
            var controller = GetCommonController();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(referenceResources);

            // Act
            var actual = await controller.GetSystemRoles();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<ReferenceResourceModel>>(jsonResult.Value);
            Assert.Equal(referenceResources, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private CommonController GetCommonController()
        {
            sessionService = FakeSessionService();
            commonService = Substitute.For<ICommonService>();
            employeeService = Substitute.For<IEmployeeService>();
            var controller = new CommonController(sessionService, commonService, employeeService);

            return controller;
        }

        #endregion Helpers
    }
}