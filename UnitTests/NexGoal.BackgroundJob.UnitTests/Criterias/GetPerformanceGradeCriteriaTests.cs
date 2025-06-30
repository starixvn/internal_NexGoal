namespace NexGoal.BackgroundJob.UnitTests.Criteria
{
    using NexGoal.BackgroundJob.Criteria;
    using Xunit;

    public class GetPerformanceGradeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetPerformanceGradeCriteria();
            var expected = "PerformanceGrade_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetPerformanceGradeCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}