namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Criteria.Common;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private IBaseService baseService;
        private IAvatarService avatarService;

        #region GetReferenceResourcesByNames

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNotNull_ReturnAsExpectedReferenceResources()
        {
            // Arrange
            var expected = GetRefResourcesForTest();
            var refNames = new[]
            {
                "SystemRole"
            };

            var service = GetCommonService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(refNames);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetReferenceResourcesByNames

        [Fact]
        public void GetSelectList_WhenCalled_ReturnAsExpectedSelectList()
        {
            // Arrange
            var refResources = GetRefResourcesForTest();
            var service = GetCommonService();

            // Act
            var actual = service.GetSelectList(
                refResources,
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value));

            // Assert
            Assert.Equal(refResources.Count, actual.Count());
        }

        [Fact]
        public async Task GetEmployeesAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, Code = "NEX", NickName = "Nexus" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<EmployeeBaseModel>(Arg.Any<SearchEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetEmployeesAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var employee = new EmployeeBaseModel { EmployeeId = 1, Code = "NEX", NickName = "Nexus" };
            var service = GetCommonService();
            baseService.GetDataEntityAsync<EmployeeBaseModel>(Arg.Any<SearchEmployeeCriteria>()).Returns(employee);
            avatarService.GetBase64Image(Arg.Any<string>()).Returns("base64string");

            // Act
            var actual = await service.GetEmployeeByIdAsync(1);

            // Assert
            Assert.Equal(employee.EmployeeId, actual.EmployeeId);
            Assert.Equal(employee.Code, actual.Code);
            Assert.Equal(employee.NickName, actual.NickName);
            Assert.Equal("base64string", actual.Base64Image);
        }

        [Fact]
        public async Task GetHRMSummary_WhenCalled_ReturnExpectedResult()
        {
            var expectResult = new HRMSummaryModel();
            var service = GetCommonService();
            baseService.GetDataEntityAsync<HRMSummaryModel>(Arg.Any<GetHRMSummaryCriteria>()).Returns(expectResult);

            var result = await service.GetHRMSummary(DateTime.Today, DateTime.Today.AddMonths(1), 363);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetGroupsAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var expected = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<GroupBaseModel>(Arg.Any<GetGroupsCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetGroupsAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetProjectsAsync_WhenCalled_ReturnAsExpectedProjects()
        {
            // Arrange
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "NexGoal" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<ProjectModel>(Arg.Any<GetProjectsCriteria>()).Returns(projects);

            // Act
            var actual = await service.GetProjectsAsync();

            // Assert
            Assert.Equal(projects, actual);
        }

        [Fact]
        public async Task GetPerformanceGradeAsync_WhenCalled_ReturnAsExpectedPerformanceGrades()
        {
            // Arrange
            var referenceResources = GetRefResourcesForTest();
            var service = GetCommonService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(referenceResources);

            // Act
            var actual = await service.GetPerformanceGradeAsync();

            // Assert
            Assert.IsAssignableFrom<IEnumerable<PerformanceGradeModel>>(actual);
            Assert.Equal(referenceResources.Count, actual.Count());
        }

        [Fact]
        public async Task GetAllPeriods_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<PeriodBaseModel>(Arg.Any<GetAllPeriodsCriteria>()).Returns(periods);

            // Act
            var actual = await service.GetAllPeriods();

            // Assert
            Assert.Equal(periods, actual);
        }

        [Fact]
        public async Task GetCurrentPerformancePeriodAsync_WhenCalled_ReturnAsExpectedPeriod()
        {
            // Arrange
            var expected = new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" };
            var service = GetCommonService();
            baseService.GetDataEntityAsync<PeriodBaseModel>(Arg.Any<GetCurrentPerformancePeriodCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetCurrentPerformancePeriodAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private List<ReferenceResourceModel> GetRefResourcesForTest()
        {
            return new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    ResourceId = 1,
                    RefName = "SystemRole",
                    Value = "SA",
                    IsActive = true
                }
            };
        }

        private CommonService GetCommonService()
        {
            baseService = Substitute.For<IBaseService>();
            avatarService = Substitute.For<IAvatarService>();
            var service = new CommonService(baseService, avatarService);

            return service;
        }

        #endregion Helpers
    }
}