namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PARecord;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NexGoal.Models.UserManagement;
    using NSubstitute;
    using Xunit;

    public class PARecordServiceTests
    {
        private IBaseService baseService;
        private IAvatarService avatarService;
        private ICryptoService cryptoService;
        private ICommonService commonService;

        [Fact]
        public async Task GetPeriodByActorAsync_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var expected = new List<PeriodModel>
            {
                new PeriodModel { PeriodId = 1, Name = "1H 2018" }
            };
            var service = GetPARecordService();
            baseService.GetDataAsync<PeriodModel>(Arg.Any<GetPeriodByActorCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetPeriodByActorAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetGroupByActorAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var expected = new List<UserGroupModel>
            {
                new UserGroupModel { GroupId = 1, Name = "Nexus" }
            };
            var service = GetPARecordService();
            baseService.GetDataAsync<UserGroupModel>(Arg.Any<GetGroupByActorCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetGroupByActorAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditPerformanceCheckInAsync_WhenCalled_ReturnAsExpectedCommandResult()
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

        #region Helpers

        private PARecordService GetPARecordService()
        {
            baseService = Substitute.For<IBaseService>();
            avatarService = Substitute.For<IAvatarService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            var service = new PARecordService(baseService, cryptoService, commonService, avatarService);

            return service;
        }

        #endregion Helpers
    }
}