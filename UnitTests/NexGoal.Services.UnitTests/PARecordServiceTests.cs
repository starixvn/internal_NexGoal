namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PARecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Repositories;
    using NSubstitute;
    using Xunit;

    public class PARecordServiceTests
    {
        private IPARepository paRepository;
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private ICommonService commonService;

        [Fact]
        public async Task GetPeriodByActorAsync_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var expected = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "1H 2018" }
            };
            var service = GetPARecordService();
            baseService.GetDataAsync<PeriodBaseModel>(Arg.Any<GetPeriodByActorCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetPeriodByActorAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetGroupByActorAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var expected = new List<Group>
            {
                new Group { GroupId = 1, Name = "Nexus" }
            };
            var service = GetPARecordService();
            baseService.GetDataAsync<Group>(Arg.Any<GetGroupByActorCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetGroupByActorAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region GetPerformanceAppraisalResultAsync

        [Fact]
        public async Task GetPerformanceAppraisalResultAsync_DataAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceAppraisals = new List<PerformanceAppraisalResult>();
            var service = GetPARecordService();
            paRepository.GetPerformanceAppraisalResultAsync(Arg.Any<GetPerformanceAppraisalResult>()).Returns(performanceAppraisals);

            // Act
            var actual = await service.GetPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetPerformanceAppraisalResultAsync_MappingData_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var performanceAppraisals = new List<PerformanceAppraisalResult>
            {
                new PerformanceAppraisalResult { PARecordId = 1, GradeId = "cipher text", Period = new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" } }
            };
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var service = GetPARecordService();
            paRepository.GetPerformanceAppraisalResultAsync(Arg.Any<GetPerformanceAppraisalResult>()).Returns(performanceAppraisals);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await service.GetPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Equal(performanceAppraisals, actual);
            Assert.Equal(performanceGrades.First(), actual.First().Grade);
        }

        #endregion GetPerformanceAppraisalResultAsync

        #region GetMemberPerformanceAppraisalResultAsync

        [Fact]
        public async Task GetMemberPerformanceAppraisalResultAsync_DataAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceAppraisals = new List<PerformanceAppraisalResult>();
            var service = GetPARecordService();
            paRepository.GetMemberPerformanceAppraisalResultAsync(Arg.Any<GetMemberPerformanceAppraisalResult>()).Returns(performanceAppraisals);

            // Act
            var actual = await service.GetMemberPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetMemberPerformanceAppraisalResultAsync_MappingData_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var performanceAppraisals = new List<PerformanceAppraisalResult>
            {
                new PerformanceAppraisalResult { PARecordId = 1, GradeId = "cipher text", Period = new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" } }
            };
            IEnumerable<PerformanceGrade> performanceGrades = Helpers.GetPerformanceGrades();
            var service = GetPARecordService();
            paRepository.GetMemberPerformanceAppraisalResultAsync(Arg.Any<GetMemberPerformanceAppraisalResult>()).Returns(performanceAppraisals);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await service.GetMemberPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Equal(performanceAppraisals, actual);
            Assert.Equal(performanceGrades.First(), actual.First().Grade);
        }

        #endregion GetMemberPerformanceAppraisalResultAsync

        [Fact]
        public async Task GetPARecordByIdAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new PARecordModel
            {
                PARecordId = 1,
                Processes = new List<PAProcessModel>()
            };

            var service = GetPARecordService();
            paRepository.GetPARecordByIdAsync(Arg.Any<GetPARecordByIdCriteria>()).Returns(expected);

            var actual = await service.GetPARecordByIdAsync(1, 363);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPARecordByReviewerAsync_WhenCalled_ReturnExpectedResult()
        {
            var gradeId = "1";
            var performanceAppraisals = new List<PARecordModel>
            {
                new PARecordModel
                {
                    PARecordId = 1,
                    Employee = new Employee { GroupName = "Nexus"},
                    GradeId = "cipher text",
                    Processes = GetPAProcess()
                }
            };

            var expected = new List<PARecordGroupModel>
            {
                new PARecordGroupModel
                {
                    GroupName = "Nexus",
                    PerformanceAppraisals = performanceAppraisals
                }
            };

            var param = new GetPARecordParam
            {
                ActorId = 363,
                PeriodId = 1
            };

            var service = GetPARecordService();

            paRepository.GetPARecordByReviewerAsync(Arg.Any<GetPARecordByReviewerCriteria>()).Returns(performanceAppraisals);
            commonService.GetPerformanceGradeAsync().Returns(Helpers.GetPerformanceGrades());
            cryptoService.Decrypt(Arg.Any<string>()).Returns(gradeId);
            var actual = await service.GetPARecordByReviewerAsync(param);

            Assert.Equal(expected.FirstOrDefault().GroupName, actual.FirstOrDefault().GroupName);
        }

        [Fact]
        public async Task EditPARecordByReviewerAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var record = new EditPARecordByReviewerModel
            {
                PARecordId = 1,
                GradeId = "E"
            };
            var service = GetPARecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPARecordByReviewerCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditPARecordByReviewerAsync(1, record);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task QuickEditPARecordByReviewerAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };

            var service = GetPARecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<QuickEditPARecordByReviewerCriteria>()).Returns(expected);

            // Act
            var actual = await service.QuickEditPARecordByReviewerAsync(165, 1, true);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPermissionOnPerformanceAppraisalAsync_WhenCalled_ReturnAsExpectedPermission()
        {
            // Arrange
            var expected = Policy.Read;
            var service = GetPARecordService();
            baseService.GetDataEntityAsync<Policy>(Arg.Any<GetPermissionOnPACriteria>()).Returns(expected);

            // Act
            var actual = await service.GetPermissionOnPerformanceAppraisalAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private IEnumerable<PAProcessModel> GetPAProcess()
        {
            var performanceRecord = new PerformanceRecord
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
            };
            var paProcess = new List<PAProcessModel>
            {
                new PAProcessModel
                {
                    PARecordId = 1,
                    RelatedPerformanceRecords = new List<PerformanceRecord> { performanceRecord }
                }
            };
            return paProcess;
        }

        private PARecordService GetPARecordService()
        {
            paRepository = Substitute.For<IPARepository>();
            baseService = Helpers.FakeBaseService();
            cryptoService = Helpers.FakeCryptoService();
            commonService = Helpers.FakeCommonService();

            var service = new PARecordService(paRepository, baseService, cryptoService, commonService);

            return service;
        }

        #endregion Helpers
    }
}