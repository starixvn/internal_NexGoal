namespace NexGoal.AutoService.UnitTests.Services
{
    using System.Collections.Generic;
    using NexGoal.AutoService.Criterias.Period;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PeriodServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public void CreatePeriod_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePeriodCriteria>()).Returns(expected);

            // Act
            var actual = service.CreatePeriod();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateEmployeeSettings_WhenCalled__ReturnAsExpectedCommandResult()
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
            var actual = service.CreateEmployeeSettings();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GeneratePA_WhenCalled__ReturnAsExpectedCommandResult()
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
            var actual = service.GeneratePA();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private PeriodService GetPeriodService()
        {
            baseService = Substitute.For<IBaseService>();

            return new PeriodService(baseService);
        }

        #endregion Helpers
    }
}