namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
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
        private INotificationPerformanceRecordService notificationService;

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
            var expectedEmployees = employees.Where(x => x.EmployeeId == 1);
            var expectedProjects = projects.Where(x => x.ProjectId == 1);
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
            Assert.Equal(performanceRecords, actual);
            Assert.True(expectedEmployees.All(x => actual.First().Employees.Any(y => y == x)));
            Assert.True(expectedProjects.All(x => actual.First().Projects.Any(y => y == x)));
            Assert.True(actual.First().Grade == performanceGrades.First());
            Assert.True(actual.First().Period == periods.First());
            Assert.True(actual.First().NominationPeriod == nominationPeriods.First());
        }

        #endregion GetPerformanceRecordByEmployeeAsync

        #region GetPerformanceTimelineAsync

        [Fact]
        public async Task GetPerformanceTimelineAsync_PerformanceRecordsAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecordModel>();
            var nominationPeriods = new List<NominationPeriodModel>();
            var tuple = new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetPerformanceTimelineCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetPerformanceTimelineAsync(new GetPerformanceTimelineParam());

            // Assert
            Assert.Empty(actual.Item1);
            Assert.Equal(0, actual.Item2);
        }

        [Fact]
        public async Task GetPerformanceTimelineAsync_MappingData_ReturnAsExpectedPerformanceRecords()
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
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var tuple = new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetPerformanceTimelineCriteria>()).Returns(tuple);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(employees);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceTimelineAsync(new GetPerformanceTimelineParam());

            // Assert
            Assert.Single(actual.Item1);
            Assert.Equal(performanceRecords, actual.Item1);
            Assert.True(actual.Item1.First().Grade == performanceGrades.First());
            Assert.True(actual.Item1.First().NominationPeriod == nominationPeriods.First());

            Assert.Equal(0, actual.Item2);
        }

        #endregion GetPerformanceTimelineAsync

        #region GetMemberPerformanceAsync

        [Fact]
        public async Task GetMemberPerformanceAsync_PerformanceRecordsAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecordModel>();
            var nominationPeriods = new List<NominationPeriodModel>();
            var tuple = new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetMemberPerformanceCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetMemberPerformanceAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetMemberPerformanceAsync_MappingData_ReturnAsExpectedPerformanceRecords()
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
                    NominationPeriodId = 1,
                    TypeId = (int)PerformanceRecordType.PerformanceCheckin
                }
            };
            IEnumerable<NominationPeriodModel> nominationPeriods = GetNominationPeriodForTest();
            IEnumerable<EmployeeBaseModel> employees = GetEmployeeForTest();
            IEnumerable<PeriodBaseModel> periods = GetPeriodForTest();
            IEnumerable<ProjectModel> projects = GetProjectForTest();
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var expectedEmployees = employees.Where(x => x.EmployeeId == 1);
            var expectedProjects = projects.Where(x => x.ProjectId == 1);
            var tuple = new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetMemberPerformanceCriteria>()).Returns(tuple);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(employees);
            commonService.GetProjectsAsync().Returns(projects);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            commonService.GetAllPeriods().Returns(periods);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetMemberPerformanceAsync(1);

            // Assert
            Assert.Single(actual);
            Assert.Equal(performanceRecords, actual);
            Assert.True(expectedEmployees.All(x => actual.First().Employees.Any(y => y == x)));
            Assert.True(expectedProjects.All(x => actual.First().Projects.Any(y => y == x)));
            Assert.True(actual.First().Grade == performanceGrades.First());
            Assert.True(actual.First().Period == periods.First());
            Assert.True(actual.First().NominationPeriod == nominationPeriods.First());
        }

        #endregion GetMemberPerformanceAsync

        #region GetMemberPerformanceTimelineAsync

        [Fact]
        public async Task GetMemberPerformanceTimelineAsync_PerformanceRecordsAreNotExist_ReturnEmpty()
        {
            // Arrange
            var memberTimelines = new List<MemberPerformanceTimelineModel>();
            var performanceRecords = new List<PerformanceRecordModel>();
            var nominationPeriods = new List<NominationPeriodModel>();
            var tuple = new Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>
                (memberTimelines, performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<MemberPerformanceTimelineModel, PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetMemberPerformanceTimelineCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetMemberPerformanceTimelineAsync(new GetMemberPerformanceTimelineParam());

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetMemberPerformanceTimelineAsync_MappingData_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            var memberTimelines = new List<MemberPerformanceTimelineModel>
            {
                new MemberPerformanceTimelineModel { EmployeeId = 1, NickName = "Tim", GradeId = "cipher text" }
            };
            var performanceRecords = new List<PerformanceRecordModel>
            {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    EmployeeIds = "1",
                    ProjectIds = "1",
                    GradeId = "cipher text",
                    NominationPeriodId = 1
                }
            };
            IEnumerable<NominationPeriodModel> nominationPeriods = GetNominationPeriodForTest();
            IEnumerable<EmployeeBaseModel> employees = GetEmployeeForTest();
            IEnumerable<ProjectModel> projects = GetProjectForTest();
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var tuple = new Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>
                (memberTimelines, performanceRecords, nominationPeriods);
            var service = GetPerformanceRecordService();
            baseService.FetchMultipleAsync<MemberPerformanceTimelineModel, PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetMemberPerformanceTimelineCriteria>()).Returns(tuple);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(employees);
            commonService.GetProjectsAsync().Returns(projects);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetMemberPerformanceTimelineAsync(new GetMemberPerformanceTimelineParam());

            // Assert
            Assert.Equal(memberTimelines, actual);
            Assert.True(actual.First().Grade == performanceGrades.First());
            Assert.True(actual.First().PerformanceRecords.First().Grade == performanceGrades.First());
            Assert.True(actual.First().PerformanceRecords.First().NominationPeriod == nominationPeriods.First());
        }

        #endregion GetMemberPerformanceTimelineAsync

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
            var expectedEmployees = employees.Where(x => x.EmployeeId == 1);
            var expectedProjects = projects.Where(x => x.ProjectId == 1);
            var service = GetPerformanceRecordService();
            baseService.GetDataEntityAsync<PerformanceRecordModel>(Arg.Any<GetPerformanceRecordByIdCriteria>()).Returns(performanceRecord);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(employees);
            commonService.GetProjectsAsync().Returns(projects);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceRecordByIdAsync(1, 1);

            // Assert
            Assert.Equal(performanceRecord, actual);
            Assert.True(expectedEmployees.All(x => actual.Employees.Any(y => y == x)));
            Assert.True(expectedProjects.All(x => actual.Projects.Any(y => y == x)));
            Assert.True(actual.Grade == performanceGrades.First());
        }

        #endregion GetPerformanceRecordByIdAsync

        #region GetPerformanceRecordByIdsAsync

        [Fact]
        public async Task GetPerformanceRecordByIdsAsync_PerformanceRecordsAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecordModel>();
            var service = GetPerformanceRecordService();
            baseService.GetDataAsync<PerformanceRecordModel>(Arg.Any<GetPerformanceRecordByIdsCriteria>()).Returns(performanceRecords);

            // Act
            var actual = await service.GetPerformanceRecordByIdsAsync("1", 1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetPerformanceRecordByIdsAsync_MappingData_ReturnAsExpectedPerformanceRecords()
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
                    GradeId = "cipher text"
                }
            };
            IEnumerable<EmployeeBaseModel> employees = GetEmployeeForTest();
            IEnumerable<ProjectModel> projects = GetProjectForTest();
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var expectedEmployees = employees.Where(x => x.EmployeeId == 1);
            var expectedProjects = projects.Where(x => x.ProjectId == 1);
            var service = GetPerformanceRecordService();
            baseService.GetDataAsync<PerformanceRecordModel>(Arg.Any<GetPerformanceRecordByIdsCriteria>()).Returns(performanceRecords);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(employees);
            commonService.GetProjectsAsync().Returns(projects);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceRecordByIdsAsync("1", 1);

            // Assert
            Assert.Equal(performanceRecords, actual);
            Assert.True(expectedEmployees.All(x => actual.First().Employees.Any(y => y == x)));
            Assert.True(expectedProjects.All(x => actual.First().Projects.Any(y => y == x)));
            Assert.True(actual.First().Grade == performanceGrades.First());
        }

        #endregion GetPerformanceRecordByIdsAsync

        #region CreatePerformanceRecordAsync

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task CreatePerformanceRecordAsync_StatusIsDraft_ReturnAsExpectedCommandResult(int resultCode)
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = resultCode
            };
            var performanceRecord = new CreatePerformanceRecordModel
            {
                EmployeeIds = "1",
                CreatorId = 2,
                StatusId = (int)PerformanceRecordStatus.Draft
            };
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreatePerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreatePerformanceRecordAsync_IsSuccessfulAndStatusIsSubmited_ExpectedSendMailAndNotification()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var performanceRecord = new CreatePerformanceRecordModel
            {
                EmployeeIds = "1",
                CreatorId = 2,
                StatusId = (int)PerformanceRecordStatus.Submitted
            };
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreatePerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
            await notificationService.Received(1).SendMailAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
            await notificationService.Received(1).SendNotificationAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
        }

        #endregion CreatePerformanceRecordAsync

        #region EditPerformanceRecordAsync

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task EditPerformanceRecordAsync_StatusIsDraft_ReturnAsExpectedCommandResult(int resultCode)
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = resultCode
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
        public async Task EditPerformanceRecordAsync_IsSuccessfulAndOriginalStatusIsDraftAndStatusIsSubmited_ExpectedSendMailAndNotification()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var originalPerformanceRecord = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2,
                StatusId = PerformanceRecordStatus.Draft
            };
            var performanceRecord = new EditPerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2,
                StatusId = (int)PerformanceRecordStatus.Submitted
            };
            var service = GetPerformanceRecordService();
            baseService.GetDataEntityAsync<PerformanceRecordModel>(Arg.Any<GetPerformanceRecordByIdCriteria>()).Returns(originalPerformanceRecord);
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditPerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
            await notificationService.Received(1).SendMailAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
            await notificationService.Received(1).SendNotificationAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
        }

        [Fact]
        public async Task EditPerformanceRecordAsync_IsSuccessfulAndOriginalStatusIsSubmitedAndStatusIsSubmited_ExpectedSendMailAndNotification()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var originalPerformanceRecord = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2,
                StatusId = PerformanceRecordStatus.Submitted
            };
            var performanceRecord = new EditPerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2,
                StatusId = (int)PerformanceRecordStatus.Submitted
            };
            var service = GetPerformanceRecordService();
            baseService.GetDataEntityAsync<PerformanceRecordModel>(Arg.Any<GetPerformanceRecordByIdCriteria>()).Returns(originalPerformanceRecord);
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditPerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
            await notificationService.Received(1).SendMailAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Edit);
            await notificationService.Received(1).SendNotificationAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Edit);
        }

        #endregion EditPerformanceRecordAsync

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
            notificationService = Substitute.For<INotificationPerformanceRecordService>();

            var service = new PerformanceRecordService(baseService, cryptoService, commonService, notificationService);

            return service;
        }

        #endregion Helpers
    }
}