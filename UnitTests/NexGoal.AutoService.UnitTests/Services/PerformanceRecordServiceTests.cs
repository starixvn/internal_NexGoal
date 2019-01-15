namespace NexGoal.AutoService.UnitTests.Services
{
    using NexGoal.AutoService.Criterias.PerformanceRecord;
    using NexGoal.AutoService.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PerformanceRecordServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public void EditStatus_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetPerformanceRecordService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditStatusCriteria>()).Returns(expected);

            // Act
            var actual = service.EditStatus();

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private PerformanceRecordService GetPerformanceRecordService()
        {
            baseService = Substitute.For<IBaseService>();

            return new PerformanceRecordService(baseService);
        }

        #endregion Helpers
    }
}