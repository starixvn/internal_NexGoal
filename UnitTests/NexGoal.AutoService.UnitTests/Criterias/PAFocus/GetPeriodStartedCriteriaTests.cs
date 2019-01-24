namespace NexGoal.AutoService.UnitTests.Criterias.PAFocus
{
    using NexGoal.AutoService.Criterias.PAFocus;
    using Xunit;

    public class GetPeriodStartedCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetPeriodStartedCriteria();
            var expected = "PAFocus_GetPeriodStarted";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetPeriodStartedCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}