namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PARecord;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Repositories;
    using NSubstitute;
    using Xunit;

    public class PARecordServiceTests
    {
        private IPARepository paRepository;
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private ICommonService commonService;
        private IAvatarService avatarService;

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
            var expected = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Name = "Nexus" }
            };
            var service = GetPARecordService();
            baseService.GetDataAsync<GroupBaseModel>(Arg.Any<GetGroupByActorCriteria>()).Returns(expected);

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
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
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
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
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

        #region Helpers

        private IEnumerable<PerformanceGradeModel> GetPerformanceGradeForTest()
        {
            var performanceGrades = new List<PerformanceGradeModel>
            {
                new PerformanceGradeModel { GradeId = 1, Name = "EE" },
                new PerformanceGradeModel { GradeId = 2, Name = "EE+" }
            };

            return performanceGrades;
        }

        private PARecordService GetPARecordService()
        {
            paRepository = Substitute.For<IPARepository>();
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            avatarService = Substitute.For<IAvatarService>();

            var service = new PARecordService(paRepository, baseService, cryptoService, commonService, avatarService);

            return service;
        }

        #endregion Helpers
    }
}