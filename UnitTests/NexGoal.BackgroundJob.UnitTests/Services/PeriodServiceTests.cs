namespace NexGoal.BackgroundJob.UnitTests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Period;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PeriodServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task CreatePeriod_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePeriodCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreatePeriodAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateEmployeeSettings_WhenCalled__ReturnAsExpectedCommandResult()
        {
            // Arrange
            var periodIds = new List<int> { 1, 2, 3 };
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPeriodService();
            baseService.GetDataAsync<int>(Arg.Any<GetPeriodStartedCriteria>()).Returns(periodIds);
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeSettingsCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreateEmployeeSettingsAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GeneratePA_WhenCalled__ReturnAsExpectedCommandResult()
        {
            // Arrange
            var periodIds = new List<int> { 1, 2, 3 };
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPeriodService();
            baseService.GetDataAsync<int>(Arg.Any<GetPeriodStartedCriteria>()).Returns(periodIds);
            baseService.ExecuteNonQueryAsync(Arg.Any<GeneratePACriteria>()).Returns(expected);

            // Act
            var actual = await service.GeneratePAAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private PeriodService GetPeriodService()
        {
            baseService = Helpers.FakeBaseService();

            return new PeriodService(baseService);
        }

        #endregion Helpers
    }
}