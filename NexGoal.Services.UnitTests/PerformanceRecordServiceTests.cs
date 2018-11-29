namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NSubstitute;
    using Xunit;

    public class PerformanceRecordServiceTests
    {
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private ICommonService commonService;

        #region GetPerformanceRecordByEmployeeAsync

        [Fact]
        public async Task GetPerformanceRecordByEmployeeAsync_PerformanceRecordsAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecordModel>();
            var nominationPeriods = new List<NominationPeriodModel>();
            var tuple = new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetPerformanceRecordByEmployeeCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetPerformanceRecordByEmployeeAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetPerformanceRecordByEmployeeAsync_MappingData_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            var performanceRecords = new List<PerformanceRecordModel>
            {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    EmployeeIds = "1",
                    ProjectIds = "1",
                    GradeId = "cipher text",
                    PeriodId = 1,
                    NominationPeriodId = 1
                }
            };
            IEnumerable<NominationPeriodModel> nominationPeriods = GetNominationPeriodForTest();
            IEnumerable<EmployeeBaseModel> employees = GetEmployeeForTest();
            IEnumerable<PeriodBaseModel> periods = GetPeriodForTest();
            IEnumerable<ProjectModel> projects = GetProjectForTest();
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var expectedEmployees = employees.Where(x => performanceRecords[0].EmployeeIds.Contains(x.EmployeeId.ToString()));
            var expectedProjects = projects.Where(x => performanceRecords[0].ProjectIds.Contains(x.ProjectId.ToString()));
            var tuple = new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetPerformanceRecordByEmployeeCriteria>()).Returns(tuple);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(employees);
            commonService.GetProjectsAsync().Returns(projects);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            commonService.GetAllPeriods().Returns(periods);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceRecordByEmployeeAsync(1);

            // Assert
            Assert.Single(actual);
            Assert.Equal(performanceRecords[0].PerformanceRecordId, actual.First().PerformanceRecordId);
            Assert.Equal(performanceRecords[0].EmployeeIds, actual.First().EmployeeIds);
            Assert.Equal(performanceRecords[0].ProjectIds, actual.First().ProjectIds);
            Assert.Equal(performanceRecords[0].GradeId, gradeId);
            Assert.Equal(performanceRecords[0].Comment, actual.First().Comment);
            Assert.Equal(performanceRecords[0].StatusId, actual.First().StatusId);
            Assert.Equal(performanceRecords[0].CreatedDate, actual.First().CreatedDate);
            Assert.Equal(performanceRecords[0].ModifiedDate, actual.First().ModifiedDate);
            Assert.Equal(performanceRecords[0].RoleId, actual.First().RoleId);
            Assert.Equal(performanceRecords[0].TypeId, actual.First().TypeId);
            Assert.Equal(performanceRecords[0].NominationStatusId, actual.First().NominationStatusId);

            Assert.Equal(performanceRecords[0].PeriodId, actual.First().PeriodId);
            Assert.Equal(performanceRecords[0].Period, actual.First().Period);

            Assert.Equal(performanceRecords[0].CreatorId, actual.First().CreatorId);
            Assert.Equal(performanceRecords[0].Creator, actual.First().Creator);

            Assert.Equal(performanceRecords[0].NominationPeriodId, actual.First().NominationPeriodId);
            Assert.Equal(performanceRecords[0].NominationPeriod, actual.First().NominationPeriod);

            Assert.True(expectedEmployees.All(x => performanceRecords[0].Employees.Any(y => y == x)));
            Assert.True(expectedProjects.All(x => performanceRecords[0].Projects.Any(y => y == x)));
            Assert.True(performanceRecords[0].Grade == performanceGrades.First());
            Assert.True(performanceRecords[0].Period == periods.First());
            Assert.True(performanceRecords[0].NominationPeriod == nominationPeriods.First());
        }

        #endregion GetPerformanceRecordByEmployeeAsync

        #region GetPerformanceRecordByIdAsync

        [Fact]
        public async Task GetPerformanceRecordByIdAsync_PerformanceRecordIsNotExist_ReturnNull()
        {
            // Arrange
            PerformanceRecordModel performanceRecord = null;
            var service = GetPerformanceRecordService();
            baseService.GetDataEntityAsync<PerformanceRecordModel>(Arg.Any<GetPerformanceRecordByIdCriteria>()).Returns(performanceRecord);

            // Act
            var actual = await service.GetPerformanceRecordByIdAsync(1, 1);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetPerformanceRecordByIdAsync_MappingData_ReturnAsExpectedPerformanceRecord()
        {
            // Arrange
            var gradeId = "1";
            var performanceRecord = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                ProjectIds = "1",
                GradeId = "cipher text"
            };
            IEnumerable<EmployeeBaseModel> employees = GetEmployeeForTest();
            IEnumerable<ProjectModel> projects = GetProjectForTest();
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var expectedEmployees = employees.Where(x => performanceRecord.EmployeeIds.Contains(x.EmployeeId.ToString()));
            var expectedProjects = projects.Where(x => performanceRecord.ProjectIds.Contains(x.ProjectId.ToString()));
            var service = GetPerformanceRecordService();
            baseService.GetDataEntityAsync<PerformanceRecordModel>(Arg.Any<GetPerformanceRecordByIdCriteria>()).Returns(performanceRecord);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(employees);
            commonService.GetProjectsAsync().Returns(projects);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceRecordByIdAsync(1, 1);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(performanceRecord.PerformanceRecordId, actual.PerformanceRecordId);
            Assert.Equal(performanceRecord.EmployeeIds, actual.EmployeeIds);
            Assert.Equal(performanceRecord.ProjectIds, actual.ProjectIds);
            Assert.Equal(performanceRecord.GradeId, gradeId);
            Assert.Equal(performanceRecord.Comment, actual.Comment);
            Assert.Equal(performanceRecord.StatusId, actual.StatusId);
            Assert.Equal(performanceRecord.CreatedDate, actual.CreatedDate);
            Assert.Equal(performanceRecord.ModifiedDate, actual.ModifiedDate);
            Assert.Equal(performanceRecord.RoleId, actual.RoleId);
            Assert.Equal(performanceRecord.TypeId, actual.TypeId);
            Assert.Equal(performanceRecord.NominationStatusId, actual.NominationStatusId);

            Assert.Equal(performanceRecord.PeriodId, actual.PeriodId);
            Assert.Equal(performanceRecord.Period, actual.Period);

            Assert.Equal(performanceRecord.CreatorId, actual.CreatorId);
            Assert.Equal(performanceRecord.Creator, actual.Creator);

            Assert.Equal(performanceRecord.NominationPeriodId, actual.NominationPeriodId);
            Assert.Equal(performanceRecord.NominationPeriod, actual.NominationPeriod);

            Assert.True(expectedEmployees.All(x => performanceRecord.Employees.Any(y => y == x)));
            Assert.True(expectedProjects.All(x => performanceRecord.Projects.Any(y => y == x)));
            Assert.True(performanceRecord.Grade == performanceGrades.First());
        }

        #endregion GetPerformanceRecordByIdAsync

        [Fact]
        public async Task CreatePerformanceRecordAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var performanceRecord = new CreatePerformanceRecordModel
            {
                EmployeeIds = "1",
                CreatorId = 2
            };
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreatePerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditPerformanceRecordAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var performanceRecord = new EditPerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2
            };
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditPerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task DeletePerformanceRecordAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<DeletePerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.DeletePerformanceRecordAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private IEnumerable<EmployeeBaseModel> GetEmployeeForTest()
        {
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" },
                new EmployeeBaseModel { EmployeeId = 2, NickName = "Quinn" }
            };

            return employees;
        }

        private IEnumerable<ProjectModel> GetProjectForTest()
        {
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "NexGoal" },
                new ProjectModel { ProjectId = 2, Name = "HRM" }
            };

            return projects;
        }

        private IEnumerable<PerformanceGradeModel> GetPerformanceGradeForTest()
        {
            var performanceGrades = new List<PerformanceGradeModel>
            {
                new PerformanceGradeModel { GradeId = 1, Name = "EE" },
                new PerformanceGradeModel { GradeId = 2, Name = "EE+" }
            };

            return performanceGrades;
        }

        private IEnumerable<PeriodBaseModel> GetPeriodForTest()
        {
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "1h 2018" },
                new PeriodBaseModel { PeriodId = 2, Name = "2h 2018" }
            };

            return periods;
        }

        private IEnumerable<NominationPeriodModel> GetNominationPeriodForTest()
        {
            var nominationPeriods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Quarter = 1, Year = 2018 },
                new NominationPeriodModel { NominationPeriodId = 2, Quarter = 2, Year = 2018 }
            };

            return nominationPeriods;
        }

        private PerformanceRecordService GetPerformanceRecordService()
        {
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();

            var service = new PerformanceRecordService(baseService, cryptoService, commonService);

            return service;
        }

        #endregion Helpers
    }
}