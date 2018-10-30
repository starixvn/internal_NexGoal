namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PerformanceRecord;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;
    using NSubstitute;
    using Xunit;

    public class PerformanceRecordServiceTests
    {
        private IBaseService baseService;
        private IAvatarService avatarService;
        private ICryptoService cryptoService;
        private ICommonService commonService;

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

        #region Helpers

        private PerformanceRecordService GetPerformanceRecordService()
        {
            baseService = Substitute.For<IBaseService>();
            avatarService = Substitute.For<IAvatarService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            var service = new PerformanceRecordService(baseService, avatarService, cryptoService, commonService);

            return service;
        }

        #endregion Helpers
    }
}