namespace NexGoal.AutoService.UnitTests.Criterias
{
    using NexGoal.AutoService.Criterias;
    using Xunit;

    public class GetPerformanceGradeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetPerformanceGradeCriteria();
            var expected = "GetPerformanceGrade";

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