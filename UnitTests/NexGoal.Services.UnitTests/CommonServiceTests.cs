namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Criteria.Common;
    using NexGoal.Criteria.Periods;
    using NexGoal.Criteria.Report;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private IBaseService baseService;
        private IMemoryCache memoryCache;

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
            var expected = new List<Employee>
            {
                new Employee { EmployeeId = 1, Code = "NEX", NickName = "Nexus" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<Employee>(Arg.Any<SearchEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetEmployeesAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, Code = "NEX", NickName = "Nexus" };
            var service = GetCommonService();
            baseService.GetDataEntityAsync<Employee>(Arg.Any<SearchEmployeeCriteria>()).Returns(employee);

            // Act
            var actual = await service.GetEmployeeByIdAsync(1);

            // Assert
            Assert.Equal(employee.EmployeeId, actual.EmployeeId);
            Assert.Equal(employee.Code, actual.Code);
            Assert.Equal(employee.NickName, actual.NickName);
        }

        [Fact]
        public async Task GetHRMSummaryAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new HRMSummaryModel { UnpaidLeave = 1 };

            var service = GetCommonService();

            baseService.GetDataEntityAsync<HRMSummaryModel>(Arg.Any<GetHRMSummaryCriteria>()).Returns(expected);

            var actual = await service.GetHRMSummaryAsync(363);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetGroupsAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var expected = new List<Group>
            {
                new Group { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<Group>(Arg.Any<GetGroupsCriteria>()).Returns(expected);

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
            var performanceGrades = Helpers.GetPerformanceGrades();
            var service = GetCommonService();
            baseService.GetDataAsync<PerformanceGrade>(Arg.Any<GetPerformanceGradeCriteria>()).Returns(performanceGrades);

            // Act
            var actual = await service.GetPerformanceGradeAsync();

            // Assert
            Assert.Equal(performanceGrades, actual);
        }

        [Fact]
        public async Task GetAllPeriodAsync_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<PeriodBaseModel>(Arg.Any<GetAllPeriodsCriteria>()).Returns(periods);

            // Act
            var actual = await service.GetAllPeriodAsync();

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

        [Fact]
        public async Task GetAnnouncementAsync_WhenCalled_ReturnAsExpectedAnnouncements()
        {
            // Arrange
            var expected = new List<string> { "announcement" };
            var criteria = new GetAnnouncementCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<string>(Arg.Do<GetAnnouncementCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await service.GetAnnouncementAsync(1);

            // Assert
            Assert.Equal(expected, actual);

            Assert.Equal(1, criteria.EmployeeId);
        }

        [Fact]
        public async Task GetPeriodForReportAsync_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var expected = new List<PeriodBaseModel> { new PeriodBaseModel { PeriodId = 1, Name = "1h 2019" } };
            var service = GetCommonService();
            baseService.GetDataAsync<PeriodBaseModel>(Arg.Any<GetPeriodForReportCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetPeriodForReportAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetProjectTagByEmployeeIdAsync_WhenCalled_ReturnAsExpectedProjects()
        {
            // Arrange
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "NexGoal" }
            };
            var criteria = new GetProjectTagByEmployeeIdCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<ProjectModel>(Arg.Do<GetProjectTagByEmployeeIdCriteria>(x => criteria = x)).Returns(projects);

            // Act
            var actual = await service.GetProjectTagByEmployeeIdAsync(1);

            // Assert
            Assert.Equal(projects, actual);
            Assert.Equal(1, criteria.EmployeeId);
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
            baseService = Helpers.FakeBaseService();
            memoryCache = Substitute.For<IMemoryCache>();
            var service = new CommonService(baseService, memoryCache);

            return service;
        }

        #endregion Helpers
    }
}