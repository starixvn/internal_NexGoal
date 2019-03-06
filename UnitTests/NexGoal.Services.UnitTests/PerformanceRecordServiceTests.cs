namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Repositories.Interfaces;
    using NSubstitute;
    using Xunit;

    public class PerformanceRecordServiceTests
    {
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private ICommonService commonService;
        private INotificationPerformanceRecordService notificationService;
        private IPerformanceRecordRepository performanceRecordRepository;
        private IBackgroundJobClient backgroundJob;

        [Fact]
        public async Task GetPerformanceRecordByEmployeeAsync_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            IEnumerable<PerformanceRecord> performanceRecords = GetPerformanceRecords();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var criteria = new GetPerformanceRecordByEmployeeCriteria();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetPerformanceRecordByEmployeeAsync(Arg.Do<GetPerformanceRecordByEmployeeCriteria>(x => criteria = x)).Returns(performanceRecords);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceRecordByEmployeeAsync(1);

            // Assert
            Assert.Equal(performanceRecords, actual);
            Assert.True(actual.First().Grade == performanceGrades.First());

            Assert.Equal(1, criteria.EmployeeId);
        }

        [Fact]
        public async Task GetPerformanceTimelineAsync_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            var param = new GetPerformanceTimelineParam();
            IEnumerable<PerformanceRecord> performanceRecords = GetPerformanceRecords();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var criteria = new GetPerformanceTimelineCriteria();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetPerformanceTimelineAsync(Arg.Do<GetPerformanceTimelineCriteria>(x => criteria = x)).Returns(performanceRecords);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceTimelineAsync(param);

            // Assert
            Assert.Equal(performanceRecords, actual.Item1);
            Assert.True(actual.Item1.First().Grade == performanceGrades.First());

            Assert.Equal(0, actual.Item2);

            Assert.Equal(param.EmployeeId, criteria.EmployeeId);
            Assert.Equal(param.PageIndex, criteria.PageIndex);
            Assert.Equal(param.PageSize, criteria.PageSize);
        }

        [Fact]
        public async Task GetMemberPerformanceAsync_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            IEnumerable<PerformanceRecord> performanceRecords = GetPerformanceRecords();
            var criteria = new GetMemberPerformanceCriteria();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetMemberPerformanceAsync(Arg.Do<GetMemberPerformanceCriteria>(x => criteria = x)).Returns(performanceRecords);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetMemberPerformanceAsync(1);

            // Assert
            Assert.Equal(performanceRecords, actual);
            Assert.True(actual.First().Grade == performanceGrades.First());

            Assert.Equal(1, criteria.ActorId);
        }

        [Fact]
        public async Task GetMemberPerformanceDetailAsync_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            IEnumerable<PerformanceRecord> performanceRecords = GetPerformanceRecords();
            var criteria = new GetMemberPerformanceDetailCriteria();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetMemberPerformanceDetailAsync(Arg.Do<GetMemberPerformanceDetailCriteria>(x => criteria = x)).Returns(performanceRecords);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetMemberPerformanceDetailAsync(1, 1);

            // Assert
            Assert.Equal(performanceRecords, actual);
            Assert.True(actual.First().Grade == performanceGrades.First());

            Assert.Equal(1, criteria.ActorId);
            Assert.Equal(1, criteria.EmployeeId);
        }

        [Fact]
        public async Task GetMemberPerformanceTimelineAsync_MappingData_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            var memberTimelines = new List<MemberPerformanceTimelineModel>
            {
                new MemberPerformanceTimelineModel { EmployeeId = 2, NickName = "tim", GradeId = "cipher text" }
            };
            IEnumerable<PerformanceRecord> performanceRecords = GetPerformanceRecords();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var tuple = new Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>(memberTimelines, performanceRecords);
            var criteria = new GetMemberPerformanceTimelineCriteria();
            var param = new GetMemberPerformanceTimelineParam();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetMemberPerformanceTimelineAsync(Arg.Do<GetMemberPerformanceTimelineCriteria>(x => criteria = x)).Returns(tuple);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetMemberPerformanceTimelineAsync(param);

            // Assert
            Assert.Equal(memberTimelines, actual);
            Assert.True(actual.First().Grade == performanceGrades.First());
            Assert.True(actual.First().PerformanceRecords.First().Grade == performanceGrades.First());
            Assert.Single(actual.First().PerformanceRecords);
            Assert.Equal(actual.First().PerformanceRecords.First(), performanceRecords.First());

            Assert.Equal(param.ActorId, criteria.ActorId);
            Assert.Equal(param.PeriodId, criteria.PeriodId);
            Assert.Equal(param.EmployeeId, criteria.EmployeeId);
            Assert.Equal(param.PageIndex, criteria.PageIndex);
            Assert.Equal(param.PageSize, criteria.PageSize);
        }

        [Fact]
        public async Task GetPerformanceRecordByIdAsync_WhenCalled_ReturnAsExpectedPerformanceRecord()
        {
            // Arrange
            var gradeId = "1";
            PerformanceRecord performanceRecord = GetPerformanceRecords().First();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var criteria = new GetPerformanceRecordByIdCriteria();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetPerformanceRecordByIdAsync(Arg.Do<GetPerformanceRecordByIdCriteria>(x => criteria = x)).Returns(performanceRecord);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceRecordByIdAsync(1, 1);

            // Assert
            Assert.Equal(performanceRecord, actual);
            Assert.True(actual.Grade == performanceGrades.First());

            Assert.Equal(1, criteria.PerformanceRecordId);
            Assert.Equal(1, criteria.CreatorId);
        }

        [Fact]
        public async Task GetDraftPerformanceRecordByIdAsync_WhenCalled_ReturnAsExpectedPerformanceRecord()
        {
            // Arrange
            var gradeId = "1";
            PerformanceRecord performanceRecord = GetPerformanceRecords().First();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var criteria = new GetDraftPerformanceRecordByIdCriteria();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetDraftPerformanceRecordByIdAsync(Arg.Do<GetDraftPerformanceRecordByIdCriteria>(x => criteria = x)).Returns(performanceRecord);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetDraftPerformanceRecordByIdAsync(1, 1);

            // Assert
            Assert.Equal(performanceRecord, actual);
            Assert.True(actual.Grade == performanceGrades.First());

            Assert.Equal(1, criteria.PerformanceRecordId);
        }

        [Fact]
        public async Task GetPerformanceRecordByIdsAsync_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var gradeId = "1";
            IEnumerable<PerformanceRecord> performanceRecords = GetPerformanceRecords();
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var criteria = new GetPerformanceRecordByIdsCriteria();
            var service = GetPerformanceRecordService();
            performanceRecordRepository.GetPerformanceRecordByIdsAsync(Arg.Do<GetPerformanceRecordByIdsCriteria>(x => criteria = x)).Returns(performanceRecords);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);

            // Act
            var actual = await service.GetPerformanceRecordByIdsAsync("1", 1);

            // Assert
            Assert.Equal(performanceRecords, actual);
            Assert.True(actual.First().Grade == performanceGrades.First());

            Assert.Equal("1", criteria.PerformanceRecordIds);
            Assert.Equal(1, criteria.CreatorId);
        }

        #region CreatePerformanceRecordAsync

        [Fact]
        public async Task CreatePerformanceRecordAsync_IsUnsuccessful_ReturnAsExpected()
        {
            // Arrange
            var expected = new CommandResult();
            var performanceRecord = new CreatePerformanceRecordDto
            {
                EmployeeIds = "1",
                CreatorId = 2
            };
            var criteria = new CreatePerformanceRecordCriteria();
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreatePerformanceRecordCriteria>(x => criteria = x)).Returns(expected);
            cryptoService.Encrypt(Arg.Any<string>()).Returns("cipher text");

            // Act
            var actual = await service.CreatePerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);

            Assert.Equal(performanceRecord.EmployeeIds, criteria.EmployeeIds);
            Assert.Equal(performanceRecord.CreatorId, criteria.CreatorId);
            Assert.Equal(performanceRecord.Comment, criteria.Comment);
            Assert.Equal(performanceRecord.ProjectIds, criteria.ProjectIds);
            Assert.Equal(performanceRecord.TypeId, criteria.TypeId);
            Assert.Equal("cipher text", criteria.GradeId);
        }

        [Fact]
        public async Task CreatePerformanceRecordAsync_IsSuccessful_ReturnAsExpected()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var performanceRecord = new CreatePerformanceRecordDto
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
            await notificationService.Received(1).SendMailAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
            await notificationService.Received(1).SendNotificationAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
        }

        #endregion CreatePerformanceRecordAsync

        [Fact]
        public async Task CreateDraftPerformanceRecordAsync_IsSuccessful_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var performanceRecord = new CreatePerformanceRecordDto
            {
                EmployeeIds = "1",
                CreatorId = 2
            };
            var criteria = new CreateDraftPerformanceRecordCriteria();
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateDraftPerformanceRecordCriteria>(x => criteria = x)).Returns(expected);
            cryptoService.Encrypt(Arg.Any<string>()).Returns("cipher text");

            // Act
            var actual = await service.CreateDraftPerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(performanceRecord.EmployeeIds, criteria.EmployeeIds);
            Assert.Equal(performanceRecord.CreatorId, criteria.CreatorId);
            Assert.Equal(performanceRecord.Comment, criteria.Comment);
            Assert.Equal(performanceRecord.ProjectIds, criteria.ProjectIds);
            Assert.Equal(performanceRecord.TypeId, criteria.TypeId);
            Assert.Equal("cipher text", criteria.GradeId);
        }

        #region EditPerformanceRecordAsync

        [Fact]
        public async Task EditPerformanceRecordAsync_IsUnsuccessful_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var performanceRecord = new EditPerformanceRecordDto
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2
            };
            var criteria = new EditPerformanceRecordCriteria();
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Do<EditPerformanceRecordCriteria>(x => criteria = x)).Returns(expected);
            cryptoService.Encrypt(Arg.Any<string>()).Returns("cipher text");

            // Act
            var actual = await service.EditPerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);

            Assert.Equal(performanceRecord.PerformanceRecordId, criteria.PerformanceRecordId);
            Assert.Equal(performanceRecord.EmployeeIds, criteria.EmployeeIds);
            Assert.Equal(performanceRecord.CreatorId, criteria.CreatorId);
            Assert.Equal(performanceRecord.Comment, criteria.Comment);
            Assert.Equal(performanceRecord.ProjectIds, criteria.ProjectIds);
            Assert.Equal(performanceRecord.TypeId, criteria.TypeId);
            Assert.Equal("cipher text", criteria.GradeId);
        }

        [Fact]
        public async Task EditPerformanceRecordAsync_IsSuccessfulAndIsFromDraft_ExpectedSendMailAndNotification()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var originalPerformanceRecord = new PerformanceRecord
            {
                PerformanceRecordId = 1
            };
            var performanceRecord = new EditPerformanceRecordDto
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2,
                Comment = "aa",
                IsFromDraft = true
            };
            var service = GetPerformanceRecordService();
            baseService.GetDataEntityAsync<PerformanceRecord>(Arg.Any<GetPerformanceRecordByIdCriteria>()).Returns(originalPerformanceRecord);
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPerformanceRecordCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditPerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
            await notificationService.Received(1).SendMailAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
            await notificationService.Received(1).SendNotificationAsync(Arg.Any<string>(), performanceRecord.CreatorId, ActionOnPerformanceRecord.Create);
        }

        [Fact]
        public async Task EditPerformanceRecordAsync_IsSuccessfulAndIsSubmited_ExpectedSendMailAndNotification()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var originalPerformanceRecord = new PerformanceRecord
            {
                PerformanceRecordId = 1
            };
            var performanceRecord = new EditPerformanceRecordDto
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2,
                Comment = "aa"
            };
            var service = GetPerformanceRecordService();
            baseService.GetDataEntityAsync<PerformanceRecord>(Arg.Any<GetPerformanceRecordByIdCriteria>()).Returns(originalPerformanceRecord);
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
        public async Task EditDraftPerformanceRecordAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var performanceRecord = new EditPerformanceRecordDto
            {
                PerformanceRecordId = 1,
                EmployeeIds = "1",
                CreatorId = 2
            };
            var criteria = new EditDraftPerformanceRecordCriteria();
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Do<EditDraftPerformanceRecordCriteria>(x => criteria = x)).Returns(expected);
            cryptoService.Encrypt(Arg.Any<string>()).Returns("cipher text");

            // Act
            var actual = await service.EditDraftPerformanceRecordAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);

            Assert.Equal(performanceRecord.PerformanceRecordId, criteria.PerformanceRecordId);
            Assert.Equal(performanceRecord.EmployeeIds, criteria.EmployeeIds);
            Assert.Equal(performanceRecord.CreatorId, criteria.CreatorId);
            Assert.Equal(performanceRecord.Comment, criteria.Comment);
            Assert.Equal(performanceRecord.ProjectIds, criteria.ProjectIds);
            Assert.Equal(performanceRecord.TypeId, criteria.TypeId);
            Assert.Equal("cipher text", criteria.GradeId);
        }

        [Fact]
        public async Task DeletePerformanceRecordAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var criteria = new DeletePerformanceRecordCriteria();
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Do<DeletePerformanceRecordCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await service.DeletePerformanceRecordAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);

            Assert.Equal(1, criteria.PerformanceRecordId);
            Assert.Equal(1, criteria.ActorId);
        }

        #region Helpers

        private IEnumerable<PerformanceRecord> GetPerformanceRecords()
        {
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    GradeId = "cipher text",
                    Comment = "comment",
                    Creator = new Employee
                    {
                        EmployeeId = 1,
                        NickName = "quinn"
                    },
                    Employees = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "tim"
                        }
                    }
                }
            };

            return performanceRecords;
        }

        private PerformanceRecordService GetPerformanceRecordService()
        {
            baseService = Helpers.FakeBaseService();
            cryptoService = Helpers.FakeCryptoService();
            commonService = Helpers.FakeCommonService();
            notificationService = Substitute.For<INotificationPerformanceRecordService>();
            performanceRecordRepository = Substitute.For<IPerformanceRecordRepository>();
            backgroundJob = Substitute.For<IBackgroundJobClient>();

            var service = new PerformanceRecordService(baseService, cryptoService, commonService, notificationService, performanceRecordRepository, backgroundJob);

            return service;
        }

        #endregion Helpers
    }
}