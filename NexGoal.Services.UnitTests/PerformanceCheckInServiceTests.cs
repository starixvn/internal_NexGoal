namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceCheckIn;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;
    using NSubstitute;
    using Xunit;

    public class PerformanceCheckInServiceTests
    {
        private IBaseService baseService;
        private IAvatarService avatarService;
        private ICryptoService cryptoService;
        private ICommonService commonService;

        [Fact]
        public async Task GetCreatorsAsync_WhenCalled_ReturnAsExpectedCreators()
        {
            // Arrange
            var expected = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "tim.trinh" }
            };
            var service = GetPerformanceCheckInService();
            baseService.GetDataAsync<EmployeeBaseModel>(Arg.Any<GetCreatorCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetCreatorsAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMembersAsync_WhenCalled_ReturnAsExpectedCreators()
        {
            // Arrange
            var expected = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "tim.trinh" }
            };
            var service = GetPerformanceCheckInService();
            baseService.GetDataAsync<EmployeeBaseModel>(Arg.Any<GetMemberCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetMembersAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreatePerformanceCheckInAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var performanceRecord = new CreatePerformanceRecordModel
            {
                EmployeeId = 1,
                CreatorId = 2
            };
            var service = GetPerformanceCheckInService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePerformanceCheckInCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreatePerformanceCheckInAsync(performanceRecord);

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
            var performanceRecord = new EditPerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeId = 1,
                CreatorId = 2
            };
            var service = GetPerformanceCheckInService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPerformanceCheckInCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditPerformanceCheckInAsync(performanceRecord);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private PerformanceCheckInService GetPerformanceCheckInService()
        {
            baseService = Substitute.For<IBaseService>();
            avatarService = Substitute.For<IAvatarService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            var service = new PerformanceCheckInService(baseService, avatarService, cryptoService, commonService);

            return service;
        }

        #endregion Helpers
    }
}